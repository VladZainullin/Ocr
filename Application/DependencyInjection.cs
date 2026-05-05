using Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2;

namespace Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<Ascii85Filter>();
        builder.Services.TryAddSingleton<AsciiHexDecodeFilter>();
        builder.Services.TryAddSingleton<CcittFaxDecodeFilter>();
        builder.Services.TryAddSingleton<DctDecodeFilter>();
        builder.Services.TryAddSingleton<FlateFilter>();
        builder.Services.TryAddSingleton<PdfboxJbig2DecodeFilter>();
        builder.Services.TryAddSingleton<JpxDecodeFilter>();
        builder.Services.TryAddSingleton<RunLengthFilter>();
        builder.Services.TryAddSingleton<LzwFilter>();
        
        builder.Services.TryAddSingleton<AppFilterProvider>();
        builder.Services.TryAddSingleton<PdfLogger>();
        builder.Services.TryAddSingleton<ParsingOptions>(static sp => new ParsingOptions
        {
            SkipMissingFonts = true,
            FilterProvider = sp.GetRequiredService<AppFilterProvider>(),
            Logger = sp.GetRequiredService<PdfLogger>()
        });
        
        builder.Services.TryAddSingleton<IPdfService, PdfService>();
        
        return builder;
    }
}