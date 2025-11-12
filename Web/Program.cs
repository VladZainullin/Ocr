using System.Net.Mime;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using Page = Tesseract.Page;

namespace Web;

file sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(static options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);

        builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        builder.Services.TryAddSingleton<ObjectPool<TesseractEngine>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = new TesseractEnginePooledObjectPolicy("./tesseract", "rus+eng");
            return provider.Create(policy);
        });

        await using var app = builder.Build();

        app.MapPost("api/v3/images", static async context =>
        {
            await using var imageStream = context.Request.Form.Files[0].OpenReadStream();

            var bytes = PreparateImage(imageStream);

            using var tesseract = new TesseractEngine("./tesseract", "eng+rus");
            var pix = Pix.LoadFromMemory(bytes);
            var page = tesseract.Process(pix);
            var blocks = ExtractLayoutFromPage(page);

            await context.Response.WriteAsJsonAsync(blocks);
        });

        app.MapPost("api/v3/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
                || context.Request.Form.Files[0].Length == 0)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    Pages = Array.Empty<PageResponse>(),
                });
                return;
            }

            var parallelOptions = new ParallelOptions
            {
                CancellationToken = context.RequestAborted,
                MaxDegreeOfParallelism = Math.Min(Math.Max(1, Environment.ProcessorCount - 1), 16),
            };

            await using var stream = context.Request.Form.Files[0].OpenReadStream();
            using var pdfDocument = PdfDocument.Open(stream);

            var pageResponses = new PageResponse[pdfDocument.NumberOfPages];
            
            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var batchSize = 100;
            for (var batchStart = 0; batchStart < pdfDocument.NumberOfPages; batchStart += batchSize)
            {
                var imageTextBuffers = new List<ImageTextBuffer>();
                var batchEnd = Math.Min(batchStart + batchSize, pdfDocument.NumberOfPages);
                for (var pdfPageNumber = batchStart + 1; pdfPageNumber <= batchEnd; pdfPageNumber++)
                {
                    var pdfPage = pdfDocument.GetPage(pdfPageNumber);

                    var words = pdfPage.GetWords(NearestNeighbourWordExtractor.Instance);
                    var blocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
                    var orderedBlocks = UnsupervisedReadingOrderDetector.Instance.Get(blocks);
                    var blockResponses = new List<BlockResponse>();
                    foreach (var block in orderedBlocks)
                    {
                        var paragraph = new BlockResponse();
                        foreach (var textLine in block.TextLines)
                        {
                            var line = new LineResponse();
                            foreach (var word in textLine.Words)
                            {
                                line.Words.Add(word.Text);
                            }

                            if (line.Words.Count > 0)
                            {
                                paragraph.Lines.Add(line);
                            }
                        }

                        if (paragraph.Lines.Count > 0)
                        {
                            blockResponses.Add(paragraph);
                        }
                    }


                    pageResponses[pdfPage.Number - 1] = new PageResponse
                    {
                        Number = pdfPage.Number,
                        Text = pdfPage.Text,
                        Blocks = blockResponses,
                    };

                    foreach (var pdfImage in pdfPage.GetImages())
                    {
                        var imageBytes = GetImageBytes(pdfImage);
                        if (imageBytes.Length == 0) continue;

                        imageTextBuffers.Add(new ImageTextBuffer
                        {
                            Number = pdfPage.Number,
                            Memory = imageBytes.ToArray(),
                        });
                    }
                }

                Parallel.ForEach(imageTextBuffers, parallelOptions, imageTextBuffer =>
                {
                    var engine = tesseractEngineObjectPool.Get();
                    try
                    {
                        var preparateImage = PreparateImage(imageTextBuffer.Memory);
                        if (preparateImage.Length == 0) return;
                        using var imageDocument = Pix.LoadFromMemory(preparateImage);
                        using var imagePage = engine.Process(imageDocument);
                        var blocks = ExtractLayoutFromPage(imagePage);
                        pageResponses[imageTextBuffer.Number - 1].Images.Add(new ImageResponse
                        {
                            Blocks = blocks,
                        });
                    }
                    finally
                    {
                        tesseractEngineObjectPool.Return(engine);
                    }
                });
            }

            await context.Response.WriteAsJsonAsync(new
            {
                Pages = pageResponses
            });
        });

        await app.RunAsync();
    }
    private static List<BlockResponse> ExtractLayoutFromPage(Page page)
    {
        var blocks = new List<BlockResponse>();
        using var iter = page.GetIterator();
        iter.Begin();

        BlockResponse? currentBlock = null;
        LineResponse? currentLine = null;

        do
        {
            if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
            {
                currentBlock = new BlockResponse();
            }

            if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine))
            {
                currentLine = new LineResponse();
            }

            if (iter.IsAtBeginningOf(PageIteratorLevel.Word))
            {
                var word = iter.GetText(PageIteratorLevel.Word);
                if (!string.IsNullOrWhiteSpace(word))
                {
                    currentLine?.Words.Add(word);
                }
            }

            if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word) && currentLine?.Words.Count > 0)
            {
                currentBlock?.Lines.Add(currentLine);
            }

            if (iter.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word) && currentBlock?.Lines.Count > 0)
            {
                blocks.Add(currentBlock);
            }
        } while (iter.Next(PageIteratorLevel.Word));

        return blocks;
    }


    private static Span<byte> GetImageBytes(IPdfImage pdfImage)
    {
        if (pdfImage.TryGetPng(out var pngImageBytes))
        {
            return pngImageBytes;
        }

        if (pdfImage.TryGetBytesAsMemory(out var memory))
        {
            return memory.Span;
        }

        return pdfImage.RawBytes;
    }

    private static byte[] PreparateImage(Span<byte> bytes)
    {
        try
        {
            using var image = new MagickImage(bytes);

            image.AutoOrient();

            image.Grayscale();

            image.MedianFilter(1);

            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }

    private static byte[] PreparateImage(Stream stream)
    {
        try
        {
            using var image = new MagickImage(stream);

            image.AutoOrient();
            image.Trim();

            image.Grayscale();

            image.MedianFilter(1);

            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }
}

public sealed class ImageResponse
{
    public required List<BlockResponse> Blocks { get; set; }
}

public sealed class BlockResponse
{
    public List<LineResponse> Lines { get; } = [];
}

public sealed class LineResponse
{
    public List<string> Words { get; } = [];
}

public sealed class PageResponse
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required List<BlockResponse> Blocks { get; set; }

    public List<ImageResponse> Images { get; } = [];
}

public sealed class ImageTextBuffer
{
    public required int Number { get; init; }

    public required byte[] Memory { get; init; }
}