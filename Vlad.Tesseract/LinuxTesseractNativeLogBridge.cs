using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Vlad.Tesseract;

internal sealed class LinuxTesseractNativeLogBridge
    : TesseractNativeLogBridgeBase
{
    private int _originalStandardError = -1;

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

        var redirected = false;

        try
        {
            LinuxNativeMethods.FlushAll();

            _originalStandardError =
                LinuxNativeMethods.Dup(StandardErrorFileDescriptor);

            if (_originalStandardError < 0)
            {
                throw CreateNativeException(
                    "dup(stderr) failed");
            }

            var pipeWriteDescriptor = checked(
                (int)PipeWriteHandle.DangerousGetHandle());

            if (LinuxNativeMethods.Dup2(
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
                LinuxNativeMethods.Dup2(
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
        LinuxNativeMethods.FlushAll();

        var originalStandardError =
            Interlocked.Exchange(
                ref _originalStandardError,
                -1);

        if (originalStandardError < 0)
        {
            return;
        }

        if (LinuxNativeMethods.Dup2(
                originalStandardError,
                StandardErrorFileDescriptor) < 0)
        {
            var exception = CreateNativeException(
                "Cannot restore stderr");

            LinuxNativeMethods.Close(originalStandardError);

            throw exception;
        }

        LinuxNativeMethods.Close(originalStandardError);
    }

    private void CloseOriginalStandardError()
    {
        var descriptor = Interlocked.Exchange(
            ref _originalStandardError,
            -1);

        if (descriptor >= 0)
        {
            LinuxNativeMethods.Close(descriptor);
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

internal static partial class LinuxNativeMethods
{
    private const string LibC = "libc.so.6";

    internal static void FlushAll()
    {
        _ = FFlush(nint.Zero);
    }

    [LibraryImport(
        LibC,
        EntryPoint = "dup",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Dup(int descriptor);

    [LibraryImport(
        LibC,
        EntryPoint = "dup2",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Dup2(
        int sourceDescriptor,
        int targetDescriptor);

    [LibraryImport(
        LibC,
        EntryPoint = "close",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Close(int descriptor);

    [LibraryImport(
        LibC,
        EntryPoint = "fflush")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    private static partial int FFlush(nint stream);
}