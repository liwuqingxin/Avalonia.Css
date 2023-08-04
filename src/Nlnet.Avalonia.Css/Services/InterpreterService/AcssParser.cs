using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nlnet.Avalonia.Css;

internal class AcssParser : IAcssParser
{
    private readonly IAcssBuilder _builder;

    public AcssParser(IAcssBuilder builder)
    {
        _builder = builder;
    }

    public ReadOnlySpan<char> RemoveCommentsAndLineBreaks(Span<char> span)
    {
        var builder = new StringBuilder();
        var index = 0;
        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '/':
                    if (Check(span, i + 1, '*'))
                    {
                        if (index != -1)
                        {
                            builder.Append(span[index..i]);
                        }
                        index = -1;
                    }
                    else if (Check(span, i - 1, '*'))
                    {
                        index = i + 1;
                    }
                    else if (Check(span, i + 1, '/'))
                    {
                        if (index != -1)
                        {
                            builder.Append(span[index..i]);
                        }
                        index = SkipTill(span, i + 1, '\r', '\n');
                        if (index > 0)
                        {
                            i = index - 1;
                        }
                    }
                    break;
                case '\r':
                case '\n':
                    span[i] = ' ';
                    break;
                default:
                    break;
            }
        }

        if (index < span.Length)
        {
            builder.Append(span[index..]);
        }

        return builder.ToString();
    }

    private static bool Check(ReadOnlySpan<char> s, int index, char ch)
    {
        if (index < 0 || index >= s.Length)
        {
            return false;
        }

        return s[index] == ch;
    }

    private static int SkipTill(Span<char> span, int cur, params char[] chars)
    {
        for (var i = cur; i < span.Length; i++)
        {
            if (chars.Contains(span[i]))
            {
                return i + 1;
            }
        }

        return -1;
    }

    public IEnumerable<(string, string)> ParseCollectionObjects(ReadOnlySpan<char> span)
    {
        var list = new List<(string, string)>();

        var index = 0;
        var selector = string.Empty;
        var leftBraceCount = 0;
        var isInStyleContent = false;
        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '[':
                    if (isInStyleContent == false)
                    {
                        isInStyleContent = true;
                        selector = span[index..i].ToString();
                        index = i + 1;
                    }
                    else
                    {
                        leftBraceCount++;
                    }
                    break;
                case ']':
                    if (leftBraceCount == 0)
                    {
                        isInStyleContent = false;
                        var content = span[index..i];
                        index = i + 1;
                        list.Add(new ValueTuple<string, string>(selector, content.ToString()));
                    }
                    else
                    {
                        leftBraceCount--;
                    }
                    break;
            }
        }

        return list;
    }

    public IEnumerable<IAcssSection> ParseSections(IAcssSection? parent, ReadOnlySpan<char> span)
    {
        var list = new List<(string, string)>();

        var index = 0;
        var selector = string.Empty;
        var leftBraceCount = 0;
        var isInStyleContent = false;
        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '{':
                    if (isInStyleContent == false)
                    {
                        isInStyleContent = true;
                        selector = span[index..i].ToString();
                        index = i + 1;
                    }
                    else
                    {
                        leftBraceCount++;
                    }
                    break;
                case '}':
                    if (leftBraceCount == 0)
                    {
                        isInStyleContent = false;
                        var content = span[index..i];
                        index = i + 1;
                        list.Add(new ValueTuple<string, string>(selector, content.ToString()));
                    }
                    else
                    {
                        leftBraceCount--;
                    }
                    break;
            }
        }

        return list.Select(o => _builder.SectionFactory.Build(this, parent, o.Item1, o.Item2));
    }
    
    public void ParseSettersAndChildren(ReadOnlySpan<char> span, out ReadOnlySpan<char> settersSpan, out ReadOnlySpan<char> childrenSpan)
    {
        var index1 = span.IndexOf("[[", StringComparison.Ordinal);
        var index2 = span.LastIndexOf("]]", StringComparison.Ordinal);

        if (index1 != -1 && index2 != -1)
        {
            var span1 = span[..(index1 - 1)];
            var span2 = span[(index2 + 2)..];
            var builder = new StringBuilder();
            builder.Append(span1);
            builder.Append(span2);

            settersSpan = builder.ToString();

            index1 += 2;
            childrenSpan = span[index1..index2];
        }
        else
        {
            settersSpan = span;
            childrenSpan = null;
        }
    }
    
    public IEnumerable<(string, string)> ParsePairs(ReadOnlySpan<char> span)
    {
        var setters = new List<(string, string)>();

        var index = 0;
        var name = string.Empty;
        string value;
        var afterColons = false;
        var isInChildCount = 0;
        var isInString = false;

        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case ':':
                    if (isInChildCount > 0 || isInString)
                    {
                        continue;
                    }
                    if (afterColons == false)
                    {
                        name = span[index..i].ToString().Trim();
                        index = i + 1;
                    }
                    afterColons = true;
                    break;
                case ';':
                    if (isInChildCount > 0 || isInString)
                    {
                        continue;
                    }
                    value = span[index..i].ToString().Trim();
                    index = i + 1;
                    setters.Add(new ValueTuple<string, string>(name, value));
                    name        = string.Empty;
                    afterColons = false;
                    break;
                case '[':
                case '{':
                    isInChildCount++;
                    break;
                case ']':
                case '}':
                    isInChildCount--;
                    if (isInChildCount > 0 || isInString)
                    {
                        continue;
                    }

                    value = span.Slice(index, i - index + 1).ToString().Trim();
                    index = i + 1;
                    setters.Add(new ValueTuple<string, string>(name, value));
                    name        = string.Empty;
                    afterColons = false;
                    break;
                case '\'':
                    isInString = !isInString;
                    break;
            }
        }

        if (index < span.Length && string.IsNullOrEmpty(name) == false)
        {
            value = span[index..].ToString().Trim(';', ' ', '\t');
            setters.Add(new ValueTuple<string, string>(name, value));
        }

        return setters;
    }
}
