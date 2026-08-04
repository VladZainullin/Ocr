using Microsoft.Extensions.Logging;

namespace Vlad.Tesseract;

internal sealed class WindowsTesseractNativeLogBridge
    : TesseractNativeLogBridgeBase
{
    private int _originalStandardError = -1;

    public WindowsTesseractNativeLogBridge(
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        if (!OperatingSystem.IsWindows())
        {
            DisposeAfterFailedConstruction();

            throw new PlatformNotSupportedException(
                "Windows native log bridge can only be used on Windows.");
        }

        var pipeWriteDescriptor = -1;
        var redirected = false;

        try
        {
            pipeWriteDescriptor =
                WindowsNativeMethods.CreatePipeWriteDescriptor(
                    PipeWriteHandle);

            WindowsNativeMethods.FlushAll();

            _originalStandardError =
                UniversalCRuntimeBase.Dup(
                    StandardErrorFileDescriptor);

            if (_originalStandardError < 0)
            {
                throw CreateCrtException(
                    "_dup(stderr) failed");
            }

            if (UniversalCRuntimeBase.Dup2(
                    pipeWriteDescriptor,
                    StandardErrorFileDescriptor) < 0)
            {
                throw CreateCrtException(
                    "_dup2(pipe, stderr) failed");
            }

            redirected = true;

            /*
             * fd=2 уже содержит собственную копию дескриптора.
             */
            _ = UniversalCRuntimeBase.Close(pipeWriteDescriptor);
            pipeWriteDescriptor = -1;

            StartReading();
        }
        catch
        {
            if (redirected &&
                _originalStandardError >= 0)
            {
                _ = UniversalCRuntimeBase.Dup2(
                    _originalStandardError,
                    StandardErrorFileDescriptor);
            }

            if (pipeWriteDescriptor >= 0)
            {
                _ = UniversalCRuntimeBase.Close(pipeWriteDescriptor);
            }

            CloseOriginalStandardError();
            DisposeAfterFailedConstruction();

            throw;
        }
    }

    protected override void RestoreStandardError()
    {
        WindowsNativeMethods.FlushAll();

        var originalStandardError =
            Interlocked.Exchange(
                ref _originalStandardError,
                -1);

        if (originalStandardError < 0)
        {
            return;
        }

        if (UniversalCRuntimeBase.Dup2(
                originalStandardError,
                StandardErrorFileDescriptor) < 0)
        {
            var exception = CreateCrtException(
                "Cannot restore stderr");

            _ = UniversalCRuntimeBase.Close(originalStandardError);

            throw exception;
        }

        _ = UniversalCRuntimeBase.Close(originalStandardError);
    }

    private void CloseOriginalStandardError()
    {
        var descriptor = Interlocked.Exchange(
            ref _originalStandardError,
            -1);

        if (descriptor >= 0)
        {
            _ = UniversalCRuntimeBase.Close(descriptor);
        }
    }

    private static IOException CreateCrtException(
        string message)
    {
        var error = WindowsNativeMethods.GetCrtError();

        return new IOException(
            $"{message}. CRT errno: {error}.");
    }
}