using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class BlockBuilder(ObjectPool<StringBuilder> stringBuilderPool) : IDisposable
{
    private readonly StringBuilder _textBuilder = stringBuilderPool.Get();
    private readonly List<LineModel> _lines = [];

    public void AddLine(LineModel line)
    {
        if (_textBuilder.Length > 0)
        {
            _textBuilder.Append(' ');
        }
        
        _textBuilder.Append(line.Text);
        
        _lines.Add(line);
    }

    public BlockModel? Build()
    {
        if (_lines.Count == 0)
        {
            return null;
        }

        var blockModel = new BlockModel
        {
            Text = _textBuilder.ToString(),
            Lines = [.. _lines]
        };

        Clear();
        return blockModel;
    }

    private void Clear()
    {
        _lines.Clear();
        _textBuilder.Clear();
    }

    public void Dispose()
    {
        _textBuilder.Clear();
        stringBuilderPool.Return(_textBuilder);
    }
}