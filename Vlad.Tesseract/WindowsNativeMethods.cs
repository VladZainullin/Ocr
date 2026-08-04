using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Vlad.Tesseract;

internal static class WindowsNativeMethods
{
    private const int OpenWriteOnly = 0x0001;
    private const int OpenBinary = 0x8000;

    private const uint DuplicateSameAccess = 0x00000002;

    internal static int CreatePipeWriteDescriptor(
        SafePipeHandle pipeHandle)
    {
        var addRef = false;
        nint duplicatedHandle = 0;

        try
        {
            pipeHandle.DangerousAddRef(ref addRef);

            var currentProcess = Kernel32.GetCurrentProcess();

            if (!Kernel32.DuplicateHandle(
                    currentProcess,
                    pipeHandle.DangerousGetHandle(),
                    currentProcess,
                    out duplicatedHandle,
                    desiredAccess: 0,
                    inheritHandle: false,
                    options: (DuplicateHandleOptions)DuplicateSameAccess))
            {
                var error = Marshal.GetLastPInvokeError();
                throw new Win32Exception(error, "DuplicateHandle(pipe) failed.");
            }
            
            var descriptor = UniversalCRuntimeBase.OpenOsFileHandle(
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
                Kernel32.CloseHandle(duplicatedHandle);
            }

            if (addRef)
            {
                pipeHandle.DangerousRelease();
            }
        }
    }

    internal static void FlushAll()
    {
        _ = UniversalCRuntimeBase.FFlush(nint.Zero);
    }

    internal static int GetCrtError()
    {
        var result = UniversalCRuntimeBase.GetErrno(out var errno);

        return result == 0
            ? errno
            : result;
    }
}