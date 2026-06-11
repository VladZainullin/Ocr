using System.Text;
using Domain;
using Microsoft.Extensions.ObjectPool;

namespace OcrService;

internal sealed class ImageBuilder(ObjectPool<StringBuilder> stringBuilderPool, int capacity = 0) : IDisposable
{
    private readonly StringBuilder _textBuilder = stringBuilderPool.Get();
    private readonly List<BlockModel> _blocks = new(capacity);

    public void AddBlock(BlockModel block)
    {
        _blocks.Add(block);

        if (_blocks.Count > 0)
        {
            _textBuilder.Append(' ');
        }
        
        _textBuilder.Append(block.Text);
    }

    public ImageModel? Build()
    {
        if (_blocks.Count == 0)
        {
            return null;
        }
        
        var image = new ImageModel
        {
            Text = _textBuilder.ToString(),
            Blocks = [.._blocks]
        };
        
        return image;
    }

    public void Clear()
    {
        _blocks.Clear();
        _textBuilder.Clear();
    }
    
    public void Dispose()
    {
        _textBuilder.Clear();
        stringBuilderPool.Return(_textBuilder);
    }
}