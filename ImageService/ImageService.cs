using ImageMagick;
using ImageService.Contracts;
using Microsoft.Extensions.Logging;

namespace ImageService;

internal sealed partial class ImageService(ILogger<ImageService> logger) : IImageService
{
    public byte[] Prepare(ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty) return [];
        
        try
        {
            using var image = new MagickImage(bytes);
            
            if (image.Width == 0 || image.Height == 0)
                return [];
            
            image.AutoOrient();
            
            image.Alpha(AlphaOption.Remove);
            image.BackgroundColor = MagickColors.White;
            
            image.ColorType = ColorType.Grayscale;
            image.Depth = 8;
            
            image.Deskew(new Percentage(10));
            
            image.Threshold(new Percentage(50));
            
            image.MedianFilter(3);
            
            image.ColorFuzz = new Percentage(10);
            image.Trim();
            image.ResetPage();
            
            image.Strip();
            
            image.Format = MagickFormat.Tiff;
            image.Settings.Compression = CompressionMethod.Group4;

            return image.ToByteArray();
        }
        catch (Exception e)
        {
            LogImageError(logger, e);
            return [];
        }
    }
    
    public byte[] Prepare(Stream stream)
    {
        if (stream.Length == 0)  return [];
        
        try
        {
            using var image = new MagickImage(stream);
            
            if (image.Width == 0 || image.Height == 0)
                return [];
            
            image.AutoOrient();
            
            image.Alpha(AlphaOption.Remove);
            image.BackgroundColor = MagickColors.White;
            
            image.ColorType = ColorType.Grayscale;
            image.Depth = 8;
            
            image.Deskew(new Percentage(10));
            
            image.Threshold(new Percentage(50));
            
            image.MedianFilter(3);
            
            image.ColorFuzz = new Percentage(10);
            image.Trim();
            image.ResetPage();
            
            image.Strip();
            
            image.Format = MagickFormat.Tiff;
            image.Settings.Compression = CompressionMethod.Group4;

            return image.ToByteArray();
        }
        catch (Exception e)
        {
            LogImageError(logger, e);
            return [];
        }
    }

    [LoggerMessage(LogLevel.Error, "Image error")]
    internal static partial void LogImageError(ILogger<ImageService> logger, Exception e);
}