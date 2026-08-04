namespace Vlad.Tesseract;

[Flags]
internal enum DuplicateHandleOptions : uint
{
    None = 0,
    CloseSource = 0x00000001,
    SameAccess = 0x00000002
}