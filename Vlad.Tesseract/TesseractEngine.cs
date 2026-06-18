using System.Runtime.InteropServices;
using Domain.Builders;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Vlad.Tesseract;

public sealed class TesseractEngine : IDisposable
{
    private readonly ObjectPool<ImageBuilder> _imageBuilderObjectPool;
    private readonly ObjectPool<BlockBuilder> _blockBuilderObjectPool;
    private readonly ObjectPool<LineBuilder> _lineBuilderObjectPool;
    private readonly IntPtr _handle;
    private bool _disposed;

    public TesseractEngine(
        string dataPath,
        string language,
        ObjectPool<ImageBuilder> imageBuilderObjectPool,
        ObjectPool<BlockBuilder> blockBuilderObjectPool,
        ObjectPool<LineBuilder> lineBuilderObjectPool)
    {
        _imageBuilderObjectPool = imageBuilderObjectPool;
        _blockBuilderObjectPool = blockBuilderObjectPool;
        _lineBuilderObjectPool = lineBuilderObjectPool;
        _handle = Native.TessBaseAPICreate();
        if (_handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to create Vlad.Tesseract API instance.");

        if (Native.TessBaseAPIInit3(_handle, dataPath, language) != 0)
        {
            Dispose();
            throw new InvalidOperationException($"Failed to initialize Vlad.Tesseract with language '{language}'.");
        }
        
        Native.TessBaseAPISetPageSegMode(_handle, PageSegMode.AutoOsd);
    }

    public unsafe string Recognize(byte[] imageData, uint width, uint height, uint bytesPerPixel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* imagePtr = imageData)
        {
            var bytesPerLine = width * bytesPerPixel;
            Native.TessBaseAPISetImage(_handle, (nint)imagePtr, width, height, bytesPerPixel, bytesPerLine);

            var textPtr = Native.TessBaseAPIGetUTF8Text(_handle);
            if (textPtr == IntPtr.Zero) return string.Empty;

            try
            {
                return Marshal.PtrToStringUTF8(textPtr) ?? string.Empty;
            }
            finally
            {
                Native.TessDeleteText(textPtr);
            }
        }
    }
    
    public unsafe ImageModel? Iterate(byte[] imageData, uint width, uint height, uint bytesPerPixel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var imageBuilder = _imageBuilderObjectPool.Get();
        var blockBuilder = _blockBuilderObjectPool.Get();
        var lineBuilder = _lineBuilderObjectPool.Get();

        try
        {
            fixed (byte* imagePtr = imageData)
            {
                var bytesPerLine = width * bytesPerPixel;
                Native.TessBaseAPISetImage(_handle, (nint)imagePtr, width, height, bytesPerPixel, bytesPerLine);

                var a = Native.TessBaseAPIRecognize(_handle, IntPtr.Zero);
                var iterator = Native.TessBaseAPIGetIterator(_handle);
                try
                {
                    do
                    {
                        var wordPtr = Native.ResultIteratorGetUTF8TextInternal(iterator, PageIteratorLevel.Word);
                        try
                        {
                            var word = Marshal.PtrToStringUTF8(wordPtr);
                            if (string.IsNullOrWhiteSpace(word))
                            {
                                continue;
                            }
                
                            lineBuilder.AddWord(word);
                        }
                        finally
                        {
                            Native.TessDeleteText(wordPtr);
                        }

                        var lastWordInLine = Native.PageIteratorIsAtFinalElement(iterator, PageIteratorLevel.Line, PageIteratorLevel.Word);
                        if (lastWordInLine != 0)
                        {
                            var lineModel = lineBuilder.Build();
                            if (lineModel != null)
                            {
                                blockBuilder.AddLine(lineModel);
                            }
                        }
                
                        var lastWordInBlock = Native.PageIteratorIsAtFinalElement(iterator, PageIteratorLevel.Block, PageIteratorLevel.Word);
                        if (lastWordInBlock != 0)
                        {
                            var blockModel = blockBuilder.Build();
                            if (blockModel != null)
                            {
                                imageBuilder.AddBlock(blockModel);
                            }
                        }
                    } while (Native.TessPageIteratorNext(iterator, PageIteratorLevel.Word));
            
                    return imageBuilder.Build();
                }
                finally
                {
                    Native.PageIteratorDelete(iterator);
                }

            }
        }
        finally
        {
            imageBuilder.Dispose();
            _imageBuilderObjectPool.Return(imageBuilder);
            blockBuilder.Dispose();
            _blockBuilderObjectPool.Return(blockBuilder);
            lineBuilder.Dispose();
            _lineBuilderObjectPool.Return(lineBuilder);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        Native.TessBaseAPIDelete(_handle);
        _disposed = true;
    }
}