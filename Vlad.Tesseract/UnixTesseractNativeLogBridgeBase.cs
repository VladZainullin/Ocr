using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Vlad.Tesseract;

internal abstract class UnixTesseractNativeLogBridgeBase
    : TesseractNativeLogBridgeBase
{
    private int _originalStandardError = -1;

    protected UnixTesseractNativeLogBridgeBase(
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
    }

    protected void RedirectStandardError()
    {
        var redirected = false;

        try
        {
            FlushAll();

            _originalStandardError =
                Dup(StandardErrorFileDescriptor);

            if (_originalStandardError < 0)
            {
                throw CreateNativeException(
                    "dup(stderr) failed");
            }

            var pipeWriteDescriptor = checked(
                (int)PipeWriteHandle.DangerousGetHandle());

            if (Dup2(
                    pipeWriteDescriptor,
                    StandardErrorFileDescriptor) < 0)
            {
                throw CreateNativeException(
                    "dup2(pipe, stderr) failed");
            }

            redirected = true;

            StartReading();
        }
        catch
        {
            if (redirected &&
                _originalStandardError >= 0)
            {
                _ = Dup2(
                    _originalStandardError,
                    StandardErrorFileDescriptor);
            }

            CloseOriginalStandardError();
            DisposeAfterFailedConstruction();

            throw;
        }
    }

    protected override void RestoreStandardError()
    {
        FlushAll();

        var originalStandardError =
            Interlocked.Exchange(
                ref _originalStandardError,
                -1);

        if (originalStandardError < 0)
        {
            return;
        }

        if (Dup2(
                originalStandardError,
                StandardErrorFileDescriptor) < 0)
        {
            var exception = CreateNativeException(
                "Cannot restore stderr");

            _ = Close(originalStandardError);

            throw exception;
        }

        _ = Close(originalStandardError);
    }

    protected abstract void FlushAll();

    protected abstract int Dup(
        int descriptor);

    protected abstract int Dup2(
        int sourceDescriptor,
        int targetDescriptor);

    protected abstract int Close(
        int descriptor);

    private void CloseOriginalStandardError()
    {
        var descriptor = Interlocked.Exchange(
            ref _originalStandardError,
            -1);

        if (descriptor >= 0)
        {
            _ = Close(descriptor);
        }
    }

    private static Win32Exception CreateNativeException(
        string message)
    {
        var error = Marshal.GetLastPInvokeError();

        return new Win32Exception(
            error,
            $"{message}. Native error: {error}.");
    }
}
