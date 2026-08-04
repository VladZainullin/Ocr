using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

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
    internal static partial int Dup(
        int descriptor);

    [LibraryImport(
        LibSystem,
        EntryPoint = "dup2",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Dup2(
        int sourceDescriptor,
        int targetDescriptor);

    [LibraryImport(
        LibSystem,
        EntryPoint = "close",
        SetLastError = true)]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Close(
        int descriptor);

    [LibraryImport(
        LibSystem,
        EntryPoint = "fflush")]
    [UnmanagedCallConv(
        CallConvs = [typeof(CallConvCdecl)])]
    private static partial int FFlush(
        nint stream);
}
