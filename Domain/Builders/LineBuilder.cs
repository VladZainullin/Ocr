using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class LineBuilder(ObjectPool<StringBuilder> stringBuilderPool) : IDisposable
{
    private readonly StringBuilder _textBuilder = stringBuilderPool.Get();
    private List<string> _words = [];

    public void AddWord(string word)
    {
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
        stringBuilderPool.Return(_textBuilder);
    }
}