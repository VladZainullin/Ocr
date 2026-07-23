using System.Text;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;

namespace Domain.Builders;

public sealed class LineBuilder : IResettable
{
    private StringBuilder? _textBuilder;
    private List<string> _words = [];

    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    internal LineBuilder(ObjectPool<StringBuilder> stringBuilderPool)
    {
        ArgumentNullException.ThrowIfNull(stringBuilderPool);

        _stringBuilderPool = stringBuilderPool;
    }

    public void AddWord(string word)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        _words.Add(word);

        var textBuilder = _textBuilder ??= _stringBuilderPool.Get();

        if (textBuilder.Length > 0)
        {
            textBuilder.Append(' ');
        }

        textBuilder.Append(word);
    }

    public LineModel? Build()
    {
        if (_words.Count == 0)
        {
            return null;
        }

        return new LineModel
        {
            Text = _textBuilder?.ToString() ?? string.Empty,
            Words = _words
        };
    }

    public bool TryReset()
    {
        _words = [];

        if (_textBuilder is null) return true;
        var textBuilder = _textBuilder;
        _textBuilder = null;

        textBuilder.Clear();
        _stringBuilderPool.Return(textBuilder);

        return true;
    }
}