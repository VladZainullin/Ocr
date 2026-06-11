using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class ImageBuilder(ObjectPool<StringBuilder> stringBuilderPool) : IDisposable
{
    private readonly StringBuilder _textBuilder = stringBuilderPool.Get();
    private List<BlockModel> _blocks = [];

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
            Blocks = _blocks
        };
        
        Clear();
        return image;
    }

    public void Clear()
    {
        _blocks = [];
        _textBuilder.Clear();
    }
    
    public void Dispose()
    {
        _textBuilder.Clear();
        stringBuilderPool.Return(_textBuilder);
    }
}