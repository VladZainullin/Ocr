using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

/// <summary>
/// Provides low-level P/Invoke bindings for the Leptonica image processing library.
/// All methods use the Cdecl calling convention.
///
/// A PIX pointer returned by Leptonica must normally be released with pixDestroy().
/// The pixDestroy() function accepts PIX**, therefore the pointer must be passed by reference.
/// </summary>
internal static partial class LeptonicaNative
{
    private const string LibraryName = "/opt/homebrew/Cellar/leptonica/1.87.0/lib/libleptonica.6.dylib";

    #region Version

    /// <summary>
    /// Returns the version string of the Leptonica library.
    /// </summary>
    /// <returns>
    /// Pointer to a null-terminated UTF-8 string.
    /// The caller must not free the returned pointer.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "getLeptonicaVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint GetLeptonicaVersion();

    #endregion

    #region Pix Lifecycle

    /// <summary>
    /// Creates a new PIX image and allocates its pixel buffer.
    /// The allocated image is initialized with zeroes.
    /// </summary>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="depth">
    /// Image depth in bits per pixel.
    /// Supported values are normally 1, 2, 4, 8, 16 and 32.
    /// </param>
    /// <returns>
    /// Pointer to the created PIX structure, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixCreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixCreate(int width, int height, int depth);

    /// <summary>
    /// Creates a PIX header without allocating an image-data buffer.
    /// </summary>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="depth">Image depth in bits per pixel.</param>
    /// <returns>
    /// Pointer to the PIX header, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixCreateHeader")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixCreateHeader(int width, int height, int depth);

    /// <summary>
    /// Creates a new PIX with the same dimensions, depth and metadata as the source image.
    /// The destination pixel buffer is initialized with zeroes.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <returns>
    /// Pointer to the created PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixCreateTemplate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixCreateTemplate(nint source);

    /// <summary>
    /// Creates another reference to the same PIX structure.
    /// This increments the PIX reference counter and does not copy image data.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <returns>
    /// Pointer to the cloned PIX reference, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixClone")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixClone(nint source);

    /// <summary>
    /// Copies a PIX image.
    /// </summary>
    /// <param name="destination">
    /// Existing destination PIX, or zero to allocate a new PIX.
    /// </param>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <returns>
    /// Pointer to the copied PIX, or zero on failure.
    /// If destination was zero, the returned PIX must be released with
    /// <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixCopy(nint destination, nint source);

    /// <summary>
    /// Decrements the PIX reference counter and destroys the image when it reaches zero.
    /// The supplied pointer is set to zero by Leptonica.
    /// </summary>
    /// <param name="pix">Reference to a pointer containing a PIX instance.</param>
    [LibraryImport(LibraryName, EntryPoint = "pixDestroy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PixDestroy(ref nint pix);

    #endregion

    #region Pix Reading

    /// <summary>
    /// Reads an image from a file.
    /// The format is detected from the file contents.
    /// </summary>
    /// <param name="filename">Path to the image file.</param>
    /// <returns>
    /// Pointer to the decoded PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(
        LibraryName,
        EntryPoint = "pixRead",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixRead(string filename);

    /// <summary>
    /// Reads an encoded image from memory.
    /// The image format is detected from the supplied data.
    /// </summary>
    /// <param name="data">
    /// Pointer to encoded image bytes, such as PNG, JPEG, TIFF, BMP or WebP.
    /// </param>
    /// <param name="size">Size of the encoded data in bytes.</param>
    /// <returns>
    /// Pointer to the decoded PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixReadMem")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixReadMem(nint data, int size);

    #endregion

    #region Pix Writing

    /// <summary>
    /// Writes a PIX image to a file.
    /// </summary>
    /// <param name="filename">Destination file path.</param>
    /// <param name="pix">Pointer to the PIX image.</param>
    /// <param name="format">Leptonica image-format identifier.</param>
    /// <returns>Zero on success; non-zero on failure.</returns>
    [LibraryImport(
        LibraryName,
        EntryPoint = "pixWrite",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixWrite(
        string filename,
        nint pix,
        LeptonicaImageFormat format);

    #endregion

    #region Pix Dimensions

    /// <summary>
    /// Gets the PIX width, height and depth.
    /// </summary>
    /// <param name="pix">Pointer to the PIX image.</param>
    /// <param name="width">Receives the image width.</param>
    /// <param name="height">Receives the image height.</param>
    /// <param name="depth">Receives the image depth.</param>
    /// <returns>Zero on success; non-zero on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "pixGetDimensions")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetDimensions(
        nint pix,
        out int width,
        out int height,
        out int depth);

    /// <summary>
    /// Returns the image width.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "pixGetWidth")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetWidth(nint pix);

    /// <summary>
    /// Returns the image height.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "pixGetHeight")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetHeight(nint pix);

    /// <summary>
    /// Returns the image depth in bits per pixel.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "pixGetDepth")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetDepth(nint pix);

    /// <summary>
    /// Returns the number of 32-bit words per image row.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "pixGetWpl")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetWordsPerLine(nint pix);

    #endregion

    #region Pix Data

    /// <summary>
    /// Returns a pointer to the internal PIX image-data buffer.
    /// </summary>
    /// <remarks>
    /// The returned pointer is owned by the PIX instance.
    /// It must not be freed separately and becomes invalid when the PIX is destroyed.
    /// Each scanline contains <see cref="PixGetWordsPerLine"/> 32-bit words.
    /// </remarks>
    [LibraryImport(LibraryName, EntryPoint = "pixGetData")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixGetData(nint pix);

    /// <summary>
    /// Replaces the internal PIX image-data pointer.
    /// </summary>
    /// <remarks>
    /// This is an unsafe ownership-transfer operation.
    /// Leptonica may later release the supplied pointer using its configured allocator.
    /// Do not pass pinned managed memory to this method.
    /// </remarks>
    [LibraryImport(LibraryName, EntryPoint = "pixSetData")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixSetData(nint pix, nint data);

    /// <summary>
    /// Gets the value of an individual pixel.
    /// </summary>
    /// <param name="pix">Pointer to the PIX image.</param>
    /// <param name="x">Horizontal pixel coordinate.</param>
    /// <param name="y">Vertical pixel coordinate.</param>
    /// <param name="value">Receives the pixel value.</param>
    /// <returns>Zero on success; non-zero on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "pixGetPixel")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetPixel(
        nint pix,
        int x,
        int y,
        out uint value);

    /// <summary>
    /// Sets the value of an individual pixel.
    /// </summary>
    /// <param name="pix">Pointer to the PIX image.</param>
    /// <param name="x">Horizontal pixel coordinate.</param>
    /// <param name="y">Vertical pixel coordinate.</param>
    /// <param name="value">New pixel value.</param>
    /// <returns>Zero on success; non-zero on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "pixSetPixel")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixSetPixel(
        nint pix,
        int x,
        int y,
        uint value);

    #endregion

    #region Pix Resolution

    /// <summary>
    /// Sets the horizontal and vertical image resolution.
    /// </summary>
    /// <param name="pix">Pointer to the PIX image.</param>
    /// <param name="xResolution">Horizontal resolution in pixels per inch.</param>
    /// <param name="yResolution">Vertical resolution in pixels per inch.</param>
    /// <returns>Zero on success; non-zero on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "pixSetResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixSetResolution(
        nint pix,
        int xResolution,
        int yResolution);

    /// <summary>
    /// Gets the horizontal image resolution in pixels per inch.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "pixGetXRes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetXResolution(nint pix);

    /// <summary>
    /// Gets the vertical image resolution in pixels per inch.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "pixGetYRes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixGetYResolution(nint pix);

    #endregion

    #region Pix Conversion

    /// <summary>
    /// Converts an image to 8 bits per pixel.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <param name="cmapFlag">
    /// Non-zero to generate a colormap when applicable; otherwise zero.
    /// </param>
    /// <returns>
    /// Pointer to the converted PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixConvertTo8")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixConvertTo8(nint source, int cmapFlag);

    /// <summary>
    /// Converts an image to 32 bits per pixel.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <returns>
    /// Pointer to the converted PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixConvertTo32")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixConvertTo32(nint source);

    /// <summary>
    /// Converts an RGB image to an 8-bit grayscale image.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <param name="redWeight">
    /// Red-channel weight. Pass 0 to use the default Leptonica weighting.
    /// </param>
    /// <param name="greenWeight">
    /// Green-channel weight. Pass 0 to use the default Leptonica weighting.
    /// </param>
    /// <param name="blueWeight">
    /// Blue-channel weight. Pass 0 to use the default Leptonica weighting.
    /// </param>
    /// <returns>
    /// Pointer to the grayscale PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixConvertRGBToGray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixConvertRgbToGray(
        nint source,
        float redWeight,
        float greenWeight,
        float blueWeight);

    /// <summary>
    /// Removes a colormap from the source image.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <param name="type">Colormap-removal mode.</param>
    /// <returns>
    /// Pointer to the converted PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixRemoveColormap")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixRemoveColormap(
        nint source,
        LeptonicaRemoveColormap type);

    #endregion

    #region Pix Binarization

    /// <summary>
    /// Converts an 8-bit grayscale image to a 1-bit binary image using a global threshold.
    /// </summary>
    /// <param name="source">Pointer to the source 8-bit PIX.</param>
    /// <param name="threshold">Threshold in the range 0 to 256.</param>
    /// <returns>
    /// Pointer to the binary PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixThresholdToBinary")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixThresholdToBinary(
        nint source,
        int threshold);

    /// <summary>
    /// Performs Otsu adaptive thresholding.
    /// </summary>
    /// <param name="source">Pointer to the source 8-bit grayscale PIX.</param>
    /// <param name="sx">
    /// Number of horizontal tiles. Use 1 for a global threshold.
    /// </param>
    /// <param name="sy">
    /// Number of vertical tiles. Use 1 for a global threshold.
    /// </param>
    /// <param name="smoothX">Horizontal smoothing half-width.</param>
    /// <param name="smoothY">Vertical smoothing half-width.</param>
    /// <param name="scoreFraction">
    /// Fraction of the maximum score used when selecting the threshold.
    /// A typical value is 0.1.
    /// </param>
    /// <param name="thresholdMap">
    /// Receives a threshold-map PIX. May be ignored by the caller, but must be
    /// destroyed when non-zero.
    /// </param>
    /// <param name="destination">
    /// Receives the binarized PIX. Must be destroyed when non-zero.
    /// </param>
    /// <returns>Zero on success; non-zero on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "pixOtsuAdaptiveThreshold")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int PixOtsuAdaptiveThreshold(
        nint source,
        int sx,
        int sy,
        int smoothX,
        int smoothY,
        float scoreFraction,
        out nint thresholdMap,
        out nint destination);

    #endregion

    #region Pix Scaling

    /// <summary>
    /// Scales an image independently in the horizontal and vertical directions.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <param name="scaleX">Horizontal scale factor.</param>
    /// <param name="scaleY">Vertical scale factor.</param>
    /// <returns>
    /// Pointer to the scaled PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixScale")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixScale(
        nint source,
        float scaleX,
        float scaleY);

    #endregion

    #region Pix Rotation and Deskew

    /// <summary>
    /// Rotates an image by a multiple of 90 degrees.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <param name="quarterTurns">
    /// Number of clockwise quarter turns.
    /// Values are normally 1, 2 or 3.
    /// </param>
    /// <returns>
    /// Pointer to the rotated PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixRotateOrth")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixRotateOrth(
        nint source,
        int quarterTurns);

    /// <summary>
    /// Estimates the skew angle and rotates the image to correct it.
    /// </summary>
    /// <param name="source">Pointer to the source PIX.</param>
    /// <param name="reduction">
    /// Reduction factor used during skew estimation.
    /// Typical values are 1, 2, 4 or 8.
    /// </param>
    /// <returns>
    /// Pointer to the deskewed PIX, or zero on failure.
    /// Must be released with <see cref="PixDestroy"/>.
    /// </returns>
    [LibraryImport(LibraryName, EntryPoint = "pixDeskew")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint PixDeskew(
        nint source,
        int reduction);

    #endregion
}

/// <summary>
/// Image format identifiers used by Leptonica.
/// Values correspond to the IFF_* constants declared by Leptonica.
/// </summary>
internal enum LeptonicaImageFormat
{
    Unknown = 0,
    Bmp = 1,
    Jpeg = 2,
    Png = 3,
    Tiff = 4,
    TiffPackbits = 5,
    TiffRle = 6,
    TiffG3 = 7,
    TiffG4 = 8,
    TiffLzw = 9,
    TiffZip = 10,
    Pnm = 11,
    Ps = 12,
    Gif = 13,
    Jp2 = 14,
    WebP = 15,
    Lpdf = 16,
    Default = 17,
    SpiX = 18
}

/// <summary>
/// Specifies how a colormap must be removed from a PIX image.
/// Values correspond to the REMOVE_CMAP_* constants declared by Leptonica.
/// </summary>
internal enum LeptonicaRemoveColormap
{
    ToBinary = 0,
    ToGray = 1,
    ToFullColor = 2,
    BasedOnSource = 3
}