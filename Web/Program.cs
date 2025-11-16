using System.Net.Mime;
using Microsoft.AspNetCore.HttpOverrides;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace Web;

file sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddWeb();

        await using var app = builder.Build();
        
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.MapPost("api/v3/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
                || context.Request.Form.Files[0].Length == 0)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    Pages = Array.Empty<Page>(),
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

            var pageResponses = new Page[pdfDocument.NumberOfPages];

            var ocr = context.RequestServices.GetRequiredService<OcrService>();
            var imageService = context.RequestServices.GetRequiredService<ImageService>();

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
                    var blockResponses = new List<Block>();
                    foreach (var block in orderedBlocks)
                    {
                        var blockResponse = new Block();
                        foreach (var textLine in block.TextLines)
                        {
                            var line = new Line();
                            foreach (var word in textLine.Words)
                            {
                                line.Words.Add(word.Text);
                            }

                            if (line.Words.Count > 0)
                            {
                                blockResponse.Lines.Add(line);
                            }
                        }

                        if (blockResponse.Lines.Count > 0)
                        {
                            blockResponses.Add(blockResponse);
                        }
                    }


                    pageResponses[pdfPage.Number - 1] = new Page
                    {
                        Number = pdfPage.Number,
                        Blocks = blockResponses,
                    };

                    foreach (var pdfImage in pdfPage.GetImages())
                    {
                        var memory = GetMemory(pdfImage);
                        if (memory.Length == 0) continue;

                        imageTextBuffers.Add(new ImageTextBuffer
                        {
                            Number = pdfPage.Number,
                            Memory = memory
                        });
                    }
                }

                Parallel.ForEach(imageTextBuffers, parallelOptions, imageTextBuffer =>
                {
                    var preparateImage = imageService.Recognition(imageTextBuffer.Memory);
                    if (preparateImage.Length == 0) return;
                    var blocks = ocr.Process(preparateImage);
                    pageResponses[imageTextBuffer.Number - 1].Images.Add(new Image
                    {
                        Blocks = blocks,
                    });
                });
            }

            await context.Response.WriteAsJsonAsync(new
            {
                Pages = pageResponses
            });
        });

        await app.RunAsync();
    }


    private static Memory<byte> GetMemory(IPdfImage pdfImage)
    {
        if (pdfImage.TryGetPng(out var pngImageBytes))
        {
            return pngImageBytes;
        }

        if (pdfImage.TryGetBytesAsMemory(out var memory))
        {
            return memory;
        }

        return pdfImage.RawMemory;
    }
}

public sealed class Image
{
    public required IEnumerable<Block> Blocks { get; set; }
}

public sealed class Block
{
    public List<Line> Lines { get; } = [];
}

public sealed class Line
{
    public List<string> Words { get; } = [];
}

public sealed class Page
{
    public required int Number { get; init; }

    public required List<Block> Blocks { get; set; }

    public List<Image> Images { get; } = [];
}

public sealed class ImageTextBuffer
{
    public required int Number { get; init; }

    public required Memory<byte> Memory { get; init; }
}