using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nlnet.Avalonia.Css;

internal class AcssParser : IAcssParser
{
    private readonly IAcssSectionFactory _sectionFactory;

    public AcssParser(IAcssSectionFactory sectionFactory)
    {
        _sectionFactory = sectionFactory;
    }

    public ReadOnlySpan<char> RemoveComments(Span<char> span)
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
                    // span[i] = ' ';
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Check(ReadOnlySpan<char> s, int index, char ch)
    {
        if (index < 0 || index >= s.Length)
        {
            return false;
        }

        return s[index] == ch;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Check(ReadOnlySpan<char> s, int index, string slice)
    {
        if (index < 0 || index + slice.Length - 1 >= s.Length)
        {
            return false;
        }

        return s.Slice(index, slice.Length).ToString() == slice;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SkipTill(ReadOnlySpan<char> span, int cur, params char[] chars)
    {
        for (var i = cur; i < span.Length; i++)
        {
            if (chars.Contains(span[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public void ParseImportsBasesAndRelies(
        ReadOnlySpan<char> span,
        out IEnumerable<string> imports,
        out IEnumerable<string> bases,
        out IEnumerable<string> relies,
        out ReadOnlySpan<char> contentSpan)
    {
        var index = 0;

        var importList = new List<string>();
        var baseList = new List<string>();
        var relyList = new List<string>();

        imports = importList;
        bases = baseList;
        relies = relyList;

        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case 'i':
                    if (ParseKeywordLine("import ", span, importList, ref i, ref index))
                    {
                        contentSpan = ReadOnlySpan<char>.Empty;
                        return;
                    }
                    break;
                case 'b':
                    if (ParseKeywordLine("base ", span, baseList, ref i, ref index))
                    {
                        contentSpan = ReadOnlySpan<char>.Empty;
                        return;
                    }
                    break;
                case 'r':
                    if (ParseKeywordLine("rely ", span, relyList, ref i, ref index))
                    {
                        contentSpan = ReadOnlySpan<char>.Empty;
                        return;
                    }
                    break;
                case '\r':
                case '\n':
                case '\t':
                case ' ':
                    index = i + 1;
                    break;
                default:
                    contentSpan = span[index..];
                    return;
            }
        }

        if (index < span.Length)
        {
            contentSpan = span[index..];
        }

        contentSpan = ReadOnlySpan<char>.Empty;
    }

    private bool ParseKeywordLine(
        string keyword,
        ReadOnlySpan<char> span, 
        ICollection<string> list,
        ref int i,
        ref int index)
    {
        if (!Check(span, i, keyword))
        {
            // continue.
            return false;
        }
        
        index = i + keyword.Length;
        var stopAt = SkipTill(span, i, ';', '\r', '\n');
        if (stopAt == -1)
        {
            // break.
            return true;
        }

        var value = span.Slice(index, stopAt - index);
        list.Add(value.ToString());

        i = index = stopAt;

        // continue.
        return false;
    }

    // TODO 是否能够和 ParseSections 合并语法？
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
                        list.Add(new ValueTuple<string, string>(selector.Trim(), content.ToString().Trim()));
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

    public IEnumerable<IAcssSection> ParseSections(AcssTokens tokens, IAcssSection? parent, ReadOnlySpan<char> span)
    {
        var list = new List<IAcssSection>();
        
        var index = 0;
        var header = string.Empty;
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
                        header = span[index..i].ToString();
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
                        list.Add( _sectionFactory.Build(this, tokens, parent, header, content));
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
    
    public void ParseSettersAndChildren(ReadOnlySpan<char> span, out ReadOnlySpan<char> settersSpan, out ReadOnlySpan<char> childrenSpan)
    {
        var index1 = span.IndexOf("[[", StringComparison.Ordinal);
        var index2 = span.LastIndexOf("]]", StringComparison.Ordinal);

        if (index1 != -1 && index2 != -1)
        {
            var span1 = span[..index1];
            
            // var span2 = span[(index2 + 2)..];
            // var builder = new StringBuilder();
            // builder.Append(span1);
            // builder.Append(span2);

            settersSpan = span1;

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
                    setters.Add(new ValueTuple<string, string>(name.Trim(), value.Trim()));
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
                    setters.Add(new ValueTuple<string, string>(name.Trim(), value.Trim()));
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
            setters.Add(new ValueTuple<string, string>(name.Trim(), value.Trim()));
        }

        return setters;
    }
}
