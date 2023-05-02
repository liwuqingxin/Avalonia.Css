using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
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

        public IStyle ToAvaloniaStyle()
        {
            var style = new Style();

            var       syntaxList = SelectorGrammar.Parse(Selector).ToList();
            Selector? selector   = null;
            var       selectors  = new List<Selector>();
            foreach (var syntax in syntaxList)
            {
                if (syntax is SelectorGrammar.CommaSyntax)
                {
                    if (selector != null)
                    {
                        selectors.Add(selector);
                    }
                    selector = null;
                }
                else
                {
                    selector = syntax.ToSelector(selector);
                }
            }
            if (selector != null)
            {
                selectors.Add(selector);
            }
            style.Selector = selectors.Count > 1 ? Selectors.Or(selectors) : selector;

            if (style.Selector?.TargetType != null)
            {
                foreach (var setter in Setters.Select(s => s.ToAvaloniaSetter(style.Selector.TargetType)).OfType<ISetter>())
                {
                    style.Add(setter);
                }
            }

            return style;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"[{Setters.Count}] '{Selector}'");
            foreach (var cssSetter in Setters)
            {
                builder.AppendLine($"    {cssSetter.ToString()}");
            }
            return builder.ToString();
        }
    }

    public class CssSetter
    {
        public string RawSetter { get; set; }

        public string? Property { get; set; }

        public string? RawValue { get; set; }

        public CssSetter(string setter)
        {
            RawSetter = setter;
            var splits = setter.Split(":", StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length == 2)
            {
                Property = splits[0];
                RawValue = splits[1];
            }
        }

        public override string ToString()
        {
            return RawSetter;
        }

        public ISetter? ToAvaloniaSetter(Type targetType)
        {
            if (Property == null)
            {
                return null;
            }

            var property = targetType.GetField($"{Property}Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                ?.GetValue(targetType) as AvaloniaProperty;
            if (property == null)
            {
                return null;
            }

            var declareType  = property.PropertyType;
            var parserMethod = declareType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new Type[] { typeof(string) });
            if (parserMethod == null)
            {
                return null;
            }

            return new Setter(property, parserMethod.Invoke(declareType, new object?[] { RawValue }));
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
