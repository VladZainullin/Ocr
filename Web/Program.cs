using Tesseract;

namespace Web;

file sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        using var engine = new TesseractEngine("./tessdata", "rus", EngineMode.Default);

        await using var app = builder.Build();

        await app.RunAsync();
    }
}