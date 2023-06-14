using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nlnet.Avalonia.Css;

public interface ICssParser
{
    /// <summary>
    /// Try parsing section objects like 'definition { setters...}'.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<(string, string)> ParseObjects(ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing sections like 'selector { content... }'. It is the same structure as section objects.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<ICssSection> ParseSections(ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing pairs like 'key:value; key2:value2...'. Note that string can be wrapped by a '.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<(string, string)> ParsePairs(ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing setters and children like 'key:value;... [[ children... ]]'.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="settersSpan"></param>
    /// <param name="childrenSpan"></param>
    /// <returns></returns>
    public void ParseSettersAndChildren(ReadOnlySpan<char> span, out ReadOnlySpan<char> settersSpan, out ReadOnlySpan<char> childrenSpan);

    /// <summary>
    /// Try removing comments like '/* ... */' from span content.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public ReadOnlySpan<char> RemoveComments(Span<char> span);

}

public class CssParser : ICssParser
{
    public IEnumerable<(string, string)> ParseObjects(ReadOnlySpan<char> span)
    {
        var list = new List<(string, string)>();

        var index            = 0;
        var selector         = string.Empty;
        var leftBraceCount   = 0;
        var isInStyleContent = false;
        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '{':
                    if (isInStyleContent == false)
                    {
                        isInStyleContent = true;
                        selector         = span[index..i].ToString();
                        index            = i + 1;
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

        return list;
    }

    public IEnumerable<ICssSection> ParseSections(ReadOnlySpan<char> span)
    {
        var objects = ParseObjects(span);

        return objects.Select(o => ServiceLocator.GetService<ICssSectionFactory>().Build(this, o.Item1, o.Item2));
    }

    public IEnumerable<(string, string)> ParsePairs(ReadOnlySpan<char> span)
    {
        var setters = new List<(string, string)>();

        var    colonsCount = 0;
        var    index       = 0;
        var    name        = string.Empty;
        string value;
        var    isInArray = false;
        var    isInString = false;

        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case ':':
                    if (isInArray || isInString)
                    {
                        continue;
                    }
                    if (colonsCount == 0)
                    {
                        name = span[index..i].ToString().Trim();
                        index = i + 1;
                    }
                    colonsCount++;
                    break;
                case ';':
                    if (isInArray || isInString)
                    {
                        continue;
                    }
                    value = span[index..i].ToString().Trim();
                    index = i + 1;
                    setters.Add(new ValueTuple<string, string>(name, value));
                    name = string.Empty;
                    colonsCount--;
                    break;
                case '[':
                    isInArray = true;
                    break;
                case ']':
                    isInArray = false;
                    value = span.Slice(index, i - index + 1).ToString().Trim();
                    index = i + 1;
                    setters.Add(new ValueTuple<string, string>(name, value));
                    name = string.Empty;
                    colonsCount--;
                    break;
                case '\'':
                    isInString = !isInString;
                    break;
            }
        }

        if (index < span.Length && string.IsNullOrEmpty(name) == false)
        {
            value = span[index..].ToString().Trim(';', ' ');
            setters.Add(new ValueTuple<string, string>(name, value));
        }

        return setters;
    }

    public void ParseSettersAndChildren(ReadOnlySpan<char> span, out ReadOnlySpan<char> settersSpan, out ReadOnlySpan<char> childrenSpan)
    {
        var index1 = span.IndexOf("[[", StringComparison.Ordinal);
        var index2 = span.LastIndexOf("]]", StringComparison.Ordinal);

        if (index1 != -1 && index2 != -1)
        {
            var span1   = span[..(index1 - 1)];
            var span2   = span[(index2   + 2)..];
            var builder = new StringBuilder();
            builder.Append(span1);
            builder.Append(span2);

            settersSpan = builder.ToString();

            index1       += 2;
            childrenSpan =  span[index1..index2];
        }
        else
        {
            settersSpan = span;
            childrenSpan = null;
        }
    }

    public ReadOnlySpan<char> RemoveComments(Span<char> span)
    {
        var builder = new StringBuilder();
        var index   = 0;
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
}
