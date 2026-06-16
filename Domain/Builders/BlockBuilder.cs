using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class BlockBuilder : IDisposable
{
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;
    private readonly StringBuilder _textBuilder;
    private List<LineModel> _lines = [];

    internal BlockBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        _stringBuilderPool = stringBuilderPool;
        _textBuilder = stringBuilderPool.Get();
    }

    public void AddLine(LineModel line)
    {
        ArgumentNullException.ThrowIfNull(line);
        
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
            Lines = _lines
        };

        Clear();
        return blockModel;
    }

    private void Clear()
    {
        _lines = [];
        _textBuilder.Clear();
    }

    public void Dispose()
    {
        _textBuilder.Clear();
        _stringBuilderPool.Return(_textBuilder);
    }
}