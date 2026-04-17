using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2;
using UglyToad.PdfPig.Tokens;

namespace Application;

internal sealed class AppFilterProvider(
    Ascii85Filter ascii85,
    AsciiHexDecodeFilter asciiHex,
    CcittFaxDecodeFilter ccitt,
    DctDecodeFilter dct,
    FlateFilter flate,
    PdfboxJbig2DecodeFilter jbig2,
    JpxDecodeFilter jpx,
    RunLengthFilter runLength,
    LzwFilter lzw) : BaseFilterProvider(new Dictionary<string, IFilter>
{
    [NameToken.Ascii85Decode.Data] = ascii85,
    [NameToken.Ascii85DecodeAbbreviation.Data] = ascii85,
    [NameToken.AsciiHexDecode.Data] = asciiHex,
    [NameToken.AsciiHexDecodeAbbreviation.Data] = asciiHex,
    [NameToken.CcittfaxDecode.Data] = ccitt,
    [NameToken.CcittfaxDecodeAbbreviation.Data] = ccitt,
    [NameToken.DctDecode.Data] = dct,
    [NameToken.DctDecodeAbbreviation.Data] = dct,
    [NameToken.FlateDecode.Data] = flate,
    [NameToken.FlateDecodeAbbreviation.Data] = flate,
    [NameToken.Jbig2Decode.Data] = jbig2,
    [NameToken.JpxDecode.Data] = jpx,
    [NameToken.RunLengthDecode.Data] = runLength,
    [NameToken.RunLengthDecodeAbbreviation.Data] = runLength,
    [NameToken.LzwDecode.Data] = lzw,
    [NameToken.LzwDecodeAbbreviation.Data] = lzw
});