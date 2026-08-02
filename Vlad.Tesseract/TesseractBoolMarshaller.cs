using System.Runtime.InteropServices.Marshalling;

namespace Vlad.Tesseract;

[CustomMarshaller(typeof(bool), MarshalMode.ManagedToUnmanagedOut, typeof(TesseractBoolMarshaller))]
internal static class TesseractBoolMarshaller
{
    public static bool ConvertToManaged(int nativeValue)
    {
        return nativeValue != 0;
    }
}