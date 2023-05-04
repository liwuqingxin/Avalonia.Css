using System;
using System.Collections.Generic;
using System.Text;

namespace Nlnet.Avalonia.Css
{
    public interface ICssParser
    {
        public IEnumerable<CssStyle> TryGetStyles(string css);

        public IEnumerable<CssSetter> TryGetSetters(string setters);
    }

    public class CssParser : ICssParser
    {
        public IEnumerable<CssStyle> TryGetStyles(string css)
        {
            css = RemoveComments(css);

            var index          = 0;
            var selector       = string.Empty;
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
                            selector         = css[index..i];
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
                            var setters = css[index..i];
                            index = i + 1;
                            yield return new CssStyle(this, selector, setters);
                        }
                        else
                        {
                            leftBraceCount--;
                        }
                        break;
                }
            }
        }

        public IEnumerable<CssSetter> TryGetSetters(string setters)
        {
            setters = setters.ReplaceLineEndings(" ");
            var settersList = setters.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (var setter in settersList)
            {
                var s = setter.Trim().TrimEnd(';');
                if (string.IsNullOrWhiteSpace(s) == false)
                {
                    yield return new CssSetter(s);
                }
            }
        }

        public static string RemoveComments(string css)
        {
            css = css.ReplaceLineEndings(" ");
            var builder = new StringBuilder();
            var index   = 0;
            for (var i = 0; i < css.Length; i++)
            {
                switch (css[i])
                {
                    case '/':
                        if (Check(css, i + 1, '*'))
                        {
                            if (index != -1)
                            {
                                builder.Append(css.AsSpan(index, i - index));
                            }
                            index = -1;
                        }
                        else if (Check(css, i - 1, '*'))
                        {
                            index = i + 1;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (index < css.Length)
            {
                builder.Append(css.AsSpan(index, css.Length - index));
            }

            return builder.ToString();
        }

        private static bool Check(string s, int index, char ch)
        {
            if (index < 0 || index >= s.Length)
            {
                return false;
            }

            return s[index] == ch;
        }
    }
}
