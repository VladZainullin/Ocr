using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

internal static partial class UniversalCRuntimeBase
{
    private const string LibraryName = "ucrtbase";
    
    [LibraryImport(LibraryName, EntryPoint = "_dup")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup(int descriptor);

    [LibraryImport(LibraryName, EntryPoint = "_dup2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup2(int sourceDescriptor, int targetDescriptor);

    [LibraryImport(LibraryName, EntryPoint = "_close")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Close(int descriptor);

    [LibraryImport(LibraryName, EntryPoint = "_open_osfhandle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int OpenOsFileHandle(nint handle, int flags);

    [LibraryImport(LibraryName, EntryPoint = "_get_errno")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetErrno(out int errno);

    [LibraryImport(LibraryName, EntryPoint = "fflush")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FFlush(nint stream);
}