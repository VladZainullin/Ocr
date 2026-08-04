using Microsoft.Extensions.Logging;

namespace Vlad.Tesseract;

internal sealed class MacOsTesseractNativeLogBridge
    : UnixTesseractNativeLogBridgeBase
{
    public MacOsTesseractNativeLogBridge(
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        if (!OperatingSystem.IsMacOS())
        {
            DisposeAfterFailedConstruction();

            throw new PlatformNotSupportedException(
                "macOS native log bridge can only be used on macOS.");
        }

        RedirectStandardError();
    }

    protected override void FlushAll()
    {
        MacOsNativeMethods.FlushAll();
    }

    protected override int Dup(
        int descriptor)
    {
        return MacOsNativeMethods.Dup(descriptor);
    }

    protected override int Dup2(
        int sourceDescriptor,
        int targetDescriptor)
    {
        return MacOsNativeMethods.Dup2(
            sourceDescriptor,
            targetDescriptor);
    }

    protected override int Close(
        int descriptor)
    {
        return MacOsNativeMethods.Close(descriptor);
    }
}
