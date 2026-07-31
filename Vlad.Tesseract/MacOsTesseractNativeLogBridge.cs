using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Vlad.Tesseract;

internal sealed class MacOsTesseractNativeLogBridge
    : TesseractNativeLogBridgeBase
{
    private int _originalStandardError = -1;

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

        var redirected = false;

        try
        {
            MacOsNativeMethods.FlushAll();

            _originalStandardError =
                MacOsNativeMethods.Dup(StandardErrorFileDescriptor);

            if (_originalStandardError < 0)
            {
                throw CreateNativeException(
                    "dup(stderr) failed");
            }

            var pipeWriteDescriptor = checked(
                (int)PipeWriteHandle.DangerousGetHandle());

            if (MacOsNativeMethods.Dup2(
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
                MacOsNativeMethods.Dup2(
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
        MacOsNativeMethods.FlushAll();

        var originalStandardError =
            Interlocked.Exchange(
                ref _originalStandardError,
                -1);

        if (originalStandardError < 0)
        {
            return;
        }

        if (MacOsNativeMethods.Dup2(
                originalStandardError,
                StandardErrorFileDescriptor) < 0)
        {
            var exception = CreateNativeException(
                "Cannot restore stderr");

            MacOsNativeMethods.Close(originalStandardError);

            throw exception;
        }

        MacOsNativeMethods.Close(originalStandardError);
    }

    private void CloseOriginalStandardError()
    {
        var descriptor = Interlocked.Exchange(
            ref _originalStandardError,
            -1);

        if (descriptor >= 0)
        {
            MacOsNativeMethods.Close(descriptor);
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

internal static partial class MacOsNativeMethods
{
    private const string LibSystem =
        "libSystem.B.dylib";

    internal static void FlushAll()
    {
        _ = FFlush(nint.Zero);
    }

    [LibraryImport(
        LibSystem,
        EntryPoint = "dup",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup(int descriptor);

    [LibraryImport(
        LibSystem,
        EntryPoint = "dup2",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup2(
        int sourceDescriptor,
        int targetDescriptor);

    [LibraryImport(
        LibSystem,
        EntryPoint = "close",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Close(int descriptor);

    [LibraryImport(
        LibSystem,
        EntryPoint = "fflush")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FFlush(nint stream);
}