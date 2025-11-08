using System.Buffers;
using System.Net.Mime;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Web;

file sealed class Program
{
    public static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

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

        app.MapPost("api/v3/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
                || context.Request.Form.Files[0].Length == 0)
            {
                await context.Response.WriteAsJsonAsync(new Response
                {
                    Pages = [],
                });
                return;
            }
            
            await using var stream = context.Request.Form.Files[0].OpenReadStream();
            using var pdfDocument = PdfDocument.Open(stream);
            
            var searchTextBuffers = new SearchTextBuffer[pdfDocument.NumberOfPages];

            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var batchSize = 100;
            for (var batchStart = 0; batchStart < pdfDocument.NumberOfPages; batchStart += batchSize)
            {
                var imageTextBuffers = new List<ImageTextBuffer>(); 
                var batchEnd = Math.Min(batchStart + batchSize, pdfDocument.NumberOfPages);
                for (var pdfPageNumber = batchStart + 1; pdfPageNumber <= batchEnd; pdfPageNumber++)
                {
                    var pdfPage = pdfDocument.GetPage(pdfPageNumber);
                    searchTextBuffers[pdfPage.Number - 1] = new SearchTextBuffer
                    {
                        Number = pdfPage.Number,
                        Text = pdfPage.Text,
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

                Parallel.ForEach(imageTextBuffers, new ParallelOptions
                {
                    CancellationToken = context.RequestAborted,
                    MaxDegreeOfParallelism = Math.Min(Math.Max(1, Environment.ProcessorCount - 1), 16),
                }, imageTextBuffer =>
                {
                    var engine = tesseractEngineObjectPool.Get();
                    try
                    {
                        using var imageDocument = Pix.LoadFromMemory(imageTextBuffer.Memory);
                        using var imagePage = engine.Process(imageDocument);
                        var text = imagePage.GetText();
                        if (text == string.Empty) return;
                        searchTextBuffers[imageTextBuffer.Number - 1].Images.Add(text);   
                    }
                    finally
                    {
                        tesseractEngineObjectPool.Return(engine);
                    } 
                });
            }
            
            await context.Response.WriteAsJsonAsync(new
            {
                Pages = searchTextBuffers
            });
        });

        // app.MapPost("api/v2/documents", static async context =>
        // {
        //     if (context.Request.Form.Files.Count < 1
        //         || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
        //         || context.Request.Form.Files[0].Length == 0)
        //     {
        //         await context.Response.WriteAsJsonAsync(new Response
        //         {
        //             Pages = [],
        //         });
        //         return;
        //     }
        //
        //     await using var stream = context.Request.Form.Files[0].OpenReadStream();
        //
        //     using var pdfDocument = PdfDocument.Open(stream);
        //
        //     var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();
        //
        //     var pdfPages = pdfDocument.GetPages();
        //     
        //     var searchTextBuffers = new SearchTextBuffer[pdfDocument.NumberOfPages];
        //     var imageTextBuffers = new List<ImageTextBuffer>();
        //     foreach (var pdfPage in pdfPages)
        //     {
        //         searchTextBuffers[pdfPage.Number - 1] = new SearchTextBuffer
        //         {
        //             Number = pdfPage.Number,
        //             Text = pdfPage.Text,
        //         };
        //
        //         var pdfImages = pdfPage.GetImages();
        //         foreach (var pdfImage in pdfImages)
        //         {
        //             var imageBytes = GetImageBytes(pdfImage);
        //             if (imageBytes.Length == 0) continue;
        //
        //             var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        //             tempFilePath = Path.ChangeExtension(tempFilePath, ".tmp");
        //
        //             await using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
        //             try
        //             {
        //                 fileStream.Write(imageBytes);
        //                 imageTextBuffers.Add(new ImageTextBuffer
        //                 {
        //                     Number = pdfPage.Number,
        //                     Memory = tempFilePath,
        //                 });
        //             }
        //             catch
        //             {
        //                 File.Delete(tempFilePath);
        //                 throw;
        //             }
        //             finally
        //             {
        //                 fileStream.Close();
        //             }
        //         }
        //     }
        //
        //     var prepareImageTextBuffers = new ImageTextBuffer[imageTextBuffers.Count];
        //     await Parallel.ForAsync(0, imageTextBuffers.Count, ParallelOptions, async (imageTextBufferIndex, cancellationToken) =>
        //     {
        //         var imageTextBuffer = imageTextBuffers[imageTextBufferIndex];
        //         await using var fileStream = new FileStream(imageTextBuffer.Memory, FileMode.Open, FileAccess.Read);
        //         try
        //         {
        //             var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        //             tempFilePath = Path.ChangeExtension(tempFilePath, ".tmp");
        //             await using var prepareFileStream = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        //             try
        //             {
        //                 try
        //                 {
        //                     using var image = new MagickImage(fileStream);
        //                     image.Grayscale();
        //                     image.Strip();
        //                     await image.WriteAsync(prepareFileStream, cancellationToken);
        //                     prepareImageTextBuffers[imageTextBufferIndex] = new ImageTextBuffer
        //                     {
        //                         Number = imageTextBuffer.Number,
        //                         Memory = tempFilePath,
        //                     };
        //                 }
        //                 catch (MagickMissingDelegateErrorException)
        //                 {
        //                     return;
        //                 }
        //             }
        //             catch
        //             {
        //                 File.Delete(tempFilePath);
        //                 throw;
        //             }
        //         }
        //         finally
        //         {
        //             File.Delete(imageTextBuffer.Memory);
        //         }
        //     });
        //     
        //     
        //     await Parallel.ForAsync(0, prepareImageTextBuffers.Length, ParallelOptions, async (imageTextBufferIndex, cancellationToken) =>
        //     {
        //         var imageTextBuffer = prepareImageTextBuffers[imageTextBufferIndex];
        //         await using var fileStream = new FileStream(imageTextBuffer.Memory, FileMode.Open, FileAccess.Read);
        //         try
        //         {
        //             var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        //             tempFilePath = Path.ChangeExtension(tempFilePath, ".tmp");
        //             await using var prepareFileStream = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        //             try
        //             {
        //                 var engine = tesseractEngineObjectPool.Get();
        //                 try
        //                 {
        //                     using var imageDocument = Pix.LoadFromFile(imageTextBuffer.Memory);
        //                     using var imagePage = engine.Process(imageDocument);
        //                     var text = imagePage.GetText();
        //                     if (text == string.Empty) return;
        //                     searchTextBuffers[imageTextBuffer.Number - 1].Images.Add(text);   
        //                 }
        //                 finally
        //                 {
        //                     tesseractEngineObjectPool.Return(engine);
        //                 }
        //             }
        //             catch
        //             {
        //                 File.Delete(tempFilePath);
        //                 throw;
        //             }
        //         }
        //         finally
        //         {
        //             File.Delete(imageTextBuffer.Memory);
        //         }
        //     });
        //
        //     await context.Response.WriteAsJsonAsync(new
        //     {
        //         Pages = searchTextBuffers,
        //     });
        // });
        //
        // app.MapPost("api/v1/documents", static async context =>
        // {
        //     if (context.Request.Form.Files.Count < 1
        //         || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
        //         || context.Request.Form.Files[0].Length == 0)
        //     {
        //         await context.Response.WriteAsJsonAsync(new Response
        //         {
        //             Pages = [],
        //         });
        //         return;
        //     }
        //
        //     await using var stream = context.Request.Form.Files[0].OpenReadStream();
        //
        //     using var pdfDocument = PdfDocument.Open(stream);
        //
        //     var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();
        //
        //     var pdfPages = pdfDocument.GetPages();
        //
        //     var pageResponses = ArrayPool<PageResponse>.Shared.Rent(pdfDocument.NumberOfPages);
        //     try
        //     {
        //         foreach (var pdfPage in pdfPages)
        //         {
        //             var pdfImages = pdfPage.GetImages();
        //             var imageResponses = new List<string>();
        //             foreach (var pdfImage in pdfImages)
        //             {
        //                 var imageBytes = GetImageBytes(pdfImage);
        //                 if (imageBytes.Length == 0) continue;
        //                 var preparateImageBytes = PreparateImage(imageBytes);
        //                 if (preparateImageBytes.Length == 0) continue;
        //                 using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
        //                 var engine = tesseractEngineObjectPool.Get();
        //                 try
        //                 {
        //                     using var imagePage = engine.Process(imageDocument);
        //                     var text = imagePage.GetText();
        //                     imageResponses.Add(text);
        //                 }
        //                 finally
        //                 {
        //                     tesseractEngineObjectPool.Return(engine);
        //                 }
        //             }
        //
        //             pageResponses[pdfPage.Number - 1] = new PageResponse
        //             {
        //                 Number = pdfPage.Number,
        //                 Text = pdfPage.Text,
        //                 Images = imageResponses
        //             };
        //         }
        //
        //         await context.Response.WriteAsJsonAsync(new Response
        //         {
        //             Pages = pageResponses,
        //         });
        //     }
        //     finally
        //     {
        //         ArrayPool<PageResponse>.Shared.Return(pageResponses);
        //     }
        // });

        await app.RunAsync();
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
            image.Grayscale();
            image.Strip();
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
            image.Grayscale();
            image.Strip();
            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }
}

public sealed class SearchTextBuffer
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public List<string> Images { get; } = [];
}

public sealed class ImageTextBuffer
{
    public required int Number { get; init; }

    public required byte[] Memory { get; init; }
}

public sealed class Response
{
    public required IReadOnlyCollection<PageResponse> Pages { get; init; }
}

public sealed class PageResponse
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required List<string> Images { get; set; }
}