using Microsoft.Extensions.Logging;

namespace Vlad.Tesseract;

internal sealed class LinuxTesseractNativeLogBridge
    : UnixTesseractNativeLogBridgeBase
{
    public LinuxTesseractNativeLogBridge(
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        if (!OperatingSystem.IsLinux())
        {
            DisposeAfterFailedConstruction();

            throw new PlatformNotSupportedException(
                "Linux native log bridge can only be used on Linux.");
        }

        RedirectStandardError();
    }

    protected override void FlushAll()
    {
        LinuxNativeMethods.FlushAll();
    }

    protected override int Dup(
        int descriptor)
    {
        return LinuxNativeMethods.Dup(descriptor);
    }

    protected override int Dup2(
        int sourceDescriptor,
        int targetDescriptor)
    {
        return LinuxNativeMethods.Dup2(
            sourceDescriptor,
            targetDescriptor);
    }

    protected override int Close(
        int descriptor)
    {
        return LinuxNativeMethods.Close(descriptor);
    }
}
