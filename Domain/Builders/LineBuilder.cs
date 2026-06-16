using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class LineBuilder : IDisposable
{
    private readonly StringBuilder _textBuilder;
    private List<string> _words = [];
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    internal LineBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        _stringBuilderPool = stringBuilderPool;
        _textBuilder = stringBuilderPool.Get();
    }

    public void AddWord(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        
        _words.Add(word);
        
        if (_textBuilder.Length > 0)
        {
            _textBuilder.Append(' ');
        }
        _textBuilder.Append(word);
    }

    public LineModel? Build()
    {
        if (_words.Count == 0)
        {
            return null;
        }

        var lineModel = new LineModel
        {
            Text = _textBuilder.ToString(),
            Words = _words
        };

        Clear();
        return lineModel;
    }

    private void Clear()
    {
        _words = [];
        _textBuilder.Clear();
    }

    public void Dispose()
    {
        _textBuilder.Clear();
        _stringBuilderPool.Return(_textBuilder);
    }
}