using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class ImageBuilder : IResettable
{
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    private StringBuilder? _textBuilder;
    private List<BlockModel> _blocks = [];

    internal ImageBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        ArgumentNullException.ThrowIfNull(stringBuilderPool);

        _stringBuilderPool = stringBuilderPool;
    }

    public void AddBlock(BlockModel block)
    {
        ArgumentNullException.ThrowIfNull(block);

        _blocks.Add(block);

        var textBuilder = _textBuilder ??= _stringBuilderPool.Get();

        if (textBuilder.Length > 0)
        {
            textBuilder.Append(' ');
        }

        textBuilder.Append(block.Text);
    }

    public ImageModel? Build()
    {
        if (_blocks.Count == 0)
        {
            return null;
        }

        var image = new ImageModel
        {
            Text = _textBuilder?.ToString() ?? string.Empty,
            Blocks = _blocks
        };

        ClearAfterBuild();

        return image;
    }

    public bool TryReset()
    {
        _blocks = [];

        if (_textBuilder is not null)
        {
            var textBuilder = _textBuilder;
            _textBuilder = null;

            textBuilder.Clear();
            _stringBuilderPool.Return(textBuilder);
        }

        return true;
    }

    private void ClearAfterBuild()
    {
        _blocks = [];
        _textBuilder?.Clear();
    }
}