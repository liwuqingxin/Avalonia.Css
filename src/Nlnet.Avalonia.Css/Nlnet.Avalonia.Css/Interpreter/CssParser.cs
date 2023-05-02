using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Nlnet.Avalonia.Css.Interpreter
{
    public class CssStyle
    {
        public string Selector { get; set; }

        public List<CssSetter> Setters { get; set; }

        public CssStyle(ICssParser parser, string selector, string setters)
        {
            Selector = selector.Trim();
            Setters = parser.TryGetSetters(setters).ToList();
        }

        public override string ToString()
        {
            return $"[{Setters.Count}] '{Selector}'";
        }
    }

    public class CssSetter
    {
        public string RawSetter { get; set; }

        public CssSetter(string setter)
        {
            RawSetter = setter;
        }

        public override string ToString()
        {
            return RawSetter;
        }
    }

    public interface ICssParser
    {
        public IEnumerable<CssStyle> TryGetStyles(string css);

        public IEnumerable<CssSetter> TryGetSetters(string setters);
    }

    internal class CssParser : ICssParser
    {
        public IEnumerable<CssStyle> TryGetStyles(string css)
        {
            css = css.ReplaceLineEndings(" ");
            var regex = new Regex("(.*?){(.*?)}");
            var matches = regex.Matches(css);
            foreach (Match match in matches)
            {
                var style    = match.Value.Trim();
                var selector = match.Groups[1].Value;
                var setters  = match.Groups[2].Value;
                yield return new CssStyle(this, selector, setters);
            }
        }

        public IEnumerable<CssSetter> TryGetSetters(string setters)
        {
            setters = setters.ReplaceLineEndings(" ");
            var setterList = setters.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (var setter in setterList)
            {
                var s = setter.Trim().TrimEnd(';');
                yield return new CssSetter(s);
            }
        }
    }

    internal class EfficientCssParser : ICssParser
    {
        public IEnumerable<CssStyle> TryGetStyles(string css)
        {
            css = RemoveComments(css);

            var index    = 0;
            var selector = string.Empty;

            for (var i = 0; i < css.Length; i++)
            {
                switch (css[i])
                {
                    case '{':
                        selector = css[index..i];
                        index    = i + 1;
                        break;
                    case '}':
                        var setters = css[index..i];
                        index   = i + 1;
                        yield return new CssStyle(this, selector, setters);
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
