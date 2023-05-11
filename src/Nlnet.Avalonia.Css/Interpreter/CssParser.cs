using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nlnet.Avalonia.Css
{
    public interface ICssParser
    {
        public IEnumerable<CssStyle> TryGetStyles();

        public IEnumerable<CssResourceList> TryGetResources();

        public IEnumerable<CssSetter> TryGetSetters(string setters);

        public ICssParser Clone(string cssContent);
    }

    internal class CssParser : ICssParser
    {
        private IList<CssStyle>? _styles;
        private IList<CssResourceList>? _resources;

        public CssParser(string cssContent)
        {
            cssContent = RemoveComments(cssContent);
            Parse(cssContent);
        }

        private void Parse(string cssContent)
        {
            _styles = new List<CssStyle>();
            _resources = new List<CssResourceList>();
            
            var index            = 0;
            var selector         = string.Empty;
            var leftBraceCount   = 0;
            var isInStyleContent = false;
            for (var i = 0; i < cssContent.Length; i++)
            {
                switch (cssContent[i])
                {
                    case '{':
                        if (isInStyleContent == false)
                        {
                            isInStyleContent = true;
                            selector         = cssContent[index..i];
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
                            var content = cssContent[index..i];
                            index = i + 1;
                            if (CssResourceList.TryGetResourceList(selector, content, out var resource))
                            {
                                _resources.Add(resource!);
                            }
                            else
                            {
                                _styles.Add(new CssStyle(this, selector, content));
                            }
                        }
                        else
                        {
                            leftBraceCount--;
                        }
                        break;
                }
            }
        }

        IEnumerable<CssStyle> ICssParser.TryGetStyles()
        {
            return _styles ?? Enumerable.Empty<CssStyle>();
        }

        IEnumerable<CssResourceList> ICssParser.TryGetResources()
        {
            return _resources ?? Enumerable.Empty<CssResourceList>();
        }

        IEnumerable<CssSetter> ICssParser.TryGetSetters(string setters)
        {
            return InterpreterHelper.ParseSetters(setters);
        }

        public ICssParser Clone(string cssContent)
        {
            return new CssParser(cssContent);
        }

        private static string RemoveComments(string css)
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
