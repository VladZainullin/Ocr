using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class BlockBuilder : IResettable
{
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    private StringBuilder? _textBuilder;
    private List<LineModel> _lines = [];

    internal BlockBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        ArgumentNullException.ThrowIfNull(stringBuilderPool);

        _stringBuilderPool = stringBuilderPool;
    }

    public void AddLine(LineModel line)
    {
        ArgumentNullException.ThrowIfNull(line);

        _lines.Add(line);

        var textBuilder = _textBuilder ??= _stringBuilderPool.Get();

        if (textBuilder.Length > 0)
        {
            textBuilder.Append(' ');
        }

        textBuilder.Append(line.Text);
    }

    public BlockModel? Build()
    {
        if (_lines.Count == 0)
        {
            return null;
        }

        var blockModel = new BlockModel
        {
            Text = _textBuilder?.ToString() ?? string.Empty,
            Lines = _lines
        };

        ClearAfterBuild();

        return blockModel;
    }

    public bool TryReset()
    {
        _lines = [];

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
        _lines = [];
        _textBuilder?.Clear();
    }
}