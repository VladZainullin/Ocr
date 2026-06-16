using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class ImageBuilder : IDisposable
{
    private readonly StringBuilder _textBuilder;
    private List<BlockModel> _blocks = [];
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    internal ImageBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        _stringBuilderPool = stringBuilderPool;
        _textBuilder = stringBuilderPool.Get();
    }

    public void AddBlock(BlockModel block)
    {
        ArgumentNullException.ThrowIfNull(block);
        
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
        _stringBuilderPool.Return(_textBuilder);
    }
}