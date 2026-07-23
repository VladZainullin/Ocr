using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class PageBuilder : IResettable
{
    private StringBuilder? _textBuilder;
    private List<ImageModel> _images = [];
    private List<BlockModel> _blocks = [];
    private int _number;

    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    internal PageBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        ArgumentNullException.ThrowIfNull(stringBuilderPool);

        _stringBuilderPool = stringBuilderPool;
    }

    public PageBuilder SetNumber(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);

        _number = number;
        return this;
    }

    public PageBuilder AddBlock(BlockModel block)
    {
        ArgumentNullException.ThrowIfNull(block);

        _blocks.Add(block);
        AppendText(block.Text);

        return this;
    }

    public PageBuilder AddImage(ImageModel image)
    {
        ArgumentNullException.ThrowIfNull(image);

        _images.Add(image);
        AppendText(image.Text);

        return this;
    }

    public PageModel Build()
    {
        return new PageModel
        {
            Number = _number,
            Images = _images,
            Blocks = _blocks,
            Text = _textBuilder?.ToString() ?? string.Empty
        };
    }

    public bool TryReset()
    {
        _number = 0;
        
        _blocks = [];
        _images = [];

        if (_textBuilder is not null)
        {
            var textBuilder = _textBuilder;
            _textBuilder = null;

            textBuilder.Clear();
            _stringBuilderPool.Return(textBuilder);
        }

        return true;
    }

    private void AppendText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var textBuilder = _textBuilder ??= _stringBuilderPool.Get();

        if (textBuilder.Length > 0)
        {
            textBuilder.Append(' ');
        }

        textBuilder.Append(text);
    }
}