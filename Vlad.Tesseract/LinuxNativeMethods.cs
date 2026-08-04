using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

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
    internal static partial int Dup(
        int descriptor);

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
    internal static partial int Close(
        int descriptor);

    [LibraryImport(
        LibC,
        EntryPoint = "fflush")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    private static partial int FFlush(
        nint stream);
}
