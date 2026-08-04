using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

internal static partial class Kernel32
{
    private const string LibraryName = "kernel32";
    
    [LibraryImport(LibraryName, EntryPoint = "GetCurrentProcess")]
    public static partial nint GetCurrentProcess();

    [LibraryImport(LibraryName, EntryPoint = "DuplicateHandle", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DuplicateHandle(
        nint sourceProcessHandle,
        nint sourceHandle,
        nint targetProcessHandle,
        out nint targetHandle,
        uint desiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
        DuplicateHandleOptions options);

    [LibraryImport(LibraryName, EntryPoint = "CloseHandle", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(nint handle);
}