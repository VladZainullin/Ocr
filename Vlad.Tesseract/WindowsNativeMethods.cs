using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Vlad.Tesseract;

[SupportedOSPlatform("windows5.1.2600")]
internal static class WindowsNativeMethods
{
    private const int OpenWriteOnly = 0x0001;
    private const int OpenBinary = 0x8000;

    internal static int CreatePipeWriteDescriptor(
        SafePipeHandle pipeHandle)
    {
        var addRef = false;
        var duplicatedHandle = HANDLE.Null;

        try
        {
            pipeHandle.DangerousAddRef(ref addRef);

            var currentProcess = PInvoke.GetCurrentProcess();

            unsafe
            {
                if (!PInvoke.DuplicateHandle(
                        currentProcess,
                        new HANDLE(pipeHandle.DangerousGetHandle()),
                        currentProcess,
                        &duplicatedHandle,
                        0,
                        false,
                        DUPLICATE_HANDLE_OPTIONS.DUPLICATE_SAME_ACCESS))
                {
                    var error = Marshal.GetLastPInvokeError();
                    throw new Win32Exception(error, "DuplicateHandle(pipe) failed.");
                }
            }
            
            var descriptor = UniversalCRuntimeBase.OpenOsFileHandle(
                duplicatedHandle,
                OpenWriteOnly | OpenBinary);

            if (descriptor >= 0)
            {
                duplicatedHandle = HANDLE.Null;
                return descriptor;
            }

            var errno = GetCrtError();

            throw new IOException(
                "_open_osfhandle(pipe) failed. " +
                $"CRT errno: {errno}.");
        }
        finally
        {
            if (!duplicatedHandle.IsNull)
            {
                PInvoke.CloseHandle(duplicatedHandle);
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
