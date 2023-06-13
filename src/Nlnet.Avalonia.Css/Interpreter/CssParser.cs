using System;
using System.Collections.Generic;
using System.Text;

namespace Nlnet.Avalonia.Css;

public interface ICssParser
{
    public IEnumerable<ICssSection> GetSections(ReadOnlySpan<char> css);
}

public class CssParser : ICssParser
{
    public IEnumerable<ICssSection> GetSections(ReadOnlySpan<char> css)
    {
        var list = new List<ICssSection>();

        var index = 0;
        var selector = string.Empty;
        var leftBraceCount = 0;
        var isInStyleContent = false;
        for (var i = 0; i < css.Length; i++)
        {
            switch (css[i])
            {
                case '{':
                    if (isInStyleContent == false)
                    {
                        isInStyleContent = true;
                        selector = css[index..i].ToString();
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
                        var content = css[index..i];
                        index = i + 1;
                        list.Add(CssSectionFactory.Build(this, selector, content));
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

    public static ReadOnlySpan<char> RemoveComments(Span<char> css)
    {
        var builder = new StringBuilder();
        var index = 0;
        for (var i = 0; i < css.Length; i++)
        {
            switch (css[i])
            {
                case '/':
                    if (Check(css, i + 1, '*'))
                    {
                        if (index != -1)
                        {
                            builder.Append(css[index..i]);
                        }
                        index = -1;
                    }
                    else if (Check(css, i - 1, '*'))
                    {
                        index = i + 1;
                    }
                    break;
                case '\r':
                case '\n':
                    css[i] = ' ';
                    break;
                default:
                    break;
            }
        }

        if (index < css.Length)
        {
            builder.Append(css[index..]);
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
