using UglyToad.PdfPig.Content;

namespace Web;

public static class PdfImageExtensions
{
    public static Memory<byte> Memory(this IPdfImage  pdfImage)
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