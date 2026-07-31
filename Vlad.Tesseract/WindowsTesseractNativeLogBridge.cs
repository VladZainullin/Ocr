using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

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
                WindowsNativeMethods.Dup(
                    StandardErrorFileDescriptor);

            if (_originalStandardError < 0)
            {
                throw CreateCrtException(
                    "_dup(stderr) failed");
            }

            if (WindowsNativeMethods.Dup2(
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
            _ = WindowsNativeMethods.Close(pipeWriteDescriptor);
            pipeWriteDescriptor = -1;

            StartReading();
        }
        catch
        {
            if (redirected &&
                _originalStandardError >= 0)
            {
                _ = WindowsNativeMethods.Dup2(
                    _originalStandardError,
                    StandardErrorFileDescriptor);
            }

            if (pipeWriteDescriptor >= 0)
            {
                _ = WindowsNativeMethods.Close(pipeWriteDescriptor);
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

        if (WindowsNativeMethods.Dup2(
                originalStandardError,
                StandardErrorFileDescriptor) < 0)
        {
            var exception = CreateCrtException(
                "Cannot restore stderr");

            _ = WindowsNativeMethods.Close(originalStandardError);

            throw exception;
        }

        _ = WindowsNativeMethods.Close(originalStandardError);
    }

    private void CloseOriginalStandardError()
    {
        var descriptor = Interlocked.Exchange(
            ref _originalStandardError,
            -1);

        if (descriptor >= 0)
        {
            _ = WindowsNativeMethods.Close(descriptor);
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

internal static partial class WindowsNativeMethods
{
    private const string Ucrt = "ucrtbase.dll";
    private const string Kernel32 = "kernel32.dll";

    private const int OpenWriteOnly = 0x0001;
    private const int OpenBinary = 0x8000;

    private const uint DuplicateSameAccess =
        0x00000002;

    internal static int CreatePipeWriteDescriptor(
        SafePipeHandle pipeHandle)
    {
        var addRef = false;
        nint duplicatedHandle = 0;

        try
        {
            pipeHandle.DangerousAddRef(ref addRef);

            var currentProcess =
                GetCurrentProcess();

            if (!DuplicateHandle(
                    currentProcess,
                    pipeHandle.DangerousGetHandle(),
                    currentProcess,
                    out duplicatedHandle,
                    desiredAccess: 0,
                    inheritHandle: false,
                    options: DuplicateSameAccess))
            {
                var error =
                    Marshal.GetLastPInvokeError();

                throw new Win32Exception(
                    error,
                    "DuplicateHandle(pipe) failed.");
            }

            /*
             * После успеха владение duplicatedHandle
             * переходит к CRT-дескриптору.
             */
            var descriptor = OpenOsFileHandle(
                duplicatedHandle,
                OpenWriteOnly | OpenBinary);

            if (descriptor >= 0)
            {
                duplicatedHandle = 0;
                return descriptor;
            }

            var errno = GetCrtError();

            throw new IOException(
                "_open_osfhandle(pipe) failed. " +
                $"CRT errno: {errno}.");
        }
        finally
        {
            if (duplicatedHandle != 0)
            {
                CloseHandle(duplicatedHandle);
            }

            if (addRef)
            {
                pipeHandle.DangerousRelease();
            }
        }
    }

    internal static void FlushAll()
    {
        _ = FFlush(nint.Zero);
    }

    internal static int GetCrtError()
    {
        var result = GetErrno(out var errno);

        return result == 0
            ? errno
            : result;
    }

    [LibraryImport(
        Ucrt,
        EntryPoint = "_dup")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup(int descriptor);

    [LibraryImport(
        Ucrt,
        EntryPoint = "_dup2")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup2(
        int sourceDescriptor,
        int targetDescriptor);

    [LibraryImport(
        Ucrt,
        EntryPoint = "_close")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Close(int descriptor);

    [LibraryImport(
        Ucrt,
        EntryPoint = "_open_osfhandle")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int OpenOsFileHandle(
        nint handle,
        int flags);

    [LibraryImport(
        Ucrt,
        EntryPoint = "_get_errno")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetErrno(
        out int errno);

    [LibraryImport(
        Ucrt,
        EntryPoint = "fflush")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FFlush(nint stream);

    [LibraryImport(
        Kernel32,
        EntryPoint = "GetCurrentProcess")]
    public static partial nint GetCurrentProcess();

    [LibraryImport(
        Kernel32,
        EntryPoint = "DuplicateHandle",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DuplicateHandle(
        nint sourceProcessHandle,
        nint sourceHandle,
        nint targetProcessHandle,
        out nint targetHandle,
        uint desiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
        uint options);

    [LibraryImport(
        Kernel32,
        EntryPoint = "CloseHandle",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(
        nint handle);
}