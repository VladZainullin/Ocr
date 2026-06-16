using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class PageBuilder : IDisposable
{
    private readonly StringBuilder _textBuilder;
    private List<ImageModel> _images = [];
    private List<BlockModel> _blocks = [];
    private int _number;
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    internal PageBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        _stringBuilderPool = stringBuilderPool;
        _textBuilder = stringBuilderPool.Get();
    }

    public PageBuilder SetNumber(int number)
    {
        if (number < 1) throw new ArgumentOutOfRangeException(nameof(number), "Number must be greater than or equal to 1.");
        
        _number = number;

        return this;
    }

    public PageBuilder AddBlock(BlockModel block)
    {
        ArgumentNullException.ThrowIfNull(block);
        
        _blocks.Add(block);

        if (_textBuilder.Length > 0)
        {
            _textBuilder.Append(' ');
        }
        
        _textBuilder.Append(block.Text);

        return this;
    }

    public PageBuilder AddImage(ImageModel image)
    {
        ArgumentNullException.ThrowIfNull(image);
        
        _images.Add(image);
        
        if (_textBuilder.Length > 0)
        {
            _textBuilder.Append(' ');
        }
        
        _textBuilder.Append(image.Text);

        return this;
    }

    public PageModel Build()
    {
        var page = new PageModel
        {
            Number = _number,
            Images = _images,
            Blocks = _blocks,
            Text = _textBuilder.ToString()
        };

        Clear();
        return page;
    }

    public PageBuilder Clear()
    {
        _images = [];
        _blocks = [];
        _textBuilder.Clear();
        return this;
    }
    
    public void Dispose()
    {
        _textBuilder.Clear();
        _stringBuilderPool.Return(_textBuilder);
    }
}