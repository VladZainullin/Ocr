using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class PageBuilder(ObjectPool<StringBuilder> stringBuilderPool) : IDisposable
{
    private readonly StringBuilder _textBuilder = stringBuilderPool.Get();
    private readonly List<ImageModel> _images = [];
    private readonly List<BlockModel> _blocks = [];
    private int _number;
    
    public PageBuilder SetNumber(int number)
    {
        _number = number;

        return this;
    }

    public PageBuilder AddBlock(BlockModel block)
    {
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
            Images = [.._images],
            Blocks = [.._blocks],
            Text = _textBuilder.ToString()
        };

        Clear();
        return page;
    }

    public PageBuilder Clear()
    {
        _images.Clear();
        _blocks.Clear();
        _textBuilder.Clear();
        return this;
    }
    
    public void Dispose()
    {
        _textBuilder.Clear();
        stringBuilderPool.Return(_textBuilder);
    }
}