using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class CssStyle
{
    private string Selector { get; set; }

    private IEnumerable<CssSetter> Setters { get; set; }

    private IEnumerable<CssStyle> Children { get; set; }

    private IEnumerable<CssResourceList> Resources { get; set; }

    public CssStyle(ICssParser parser, string selector, string contentString)
    {
        Selector = selector.Trim();

        var index1 = contentString.IndexOf("[[", StringComparison.Ordinal);
        var index2 = contentString.LastIndexOf("]]", StringComparison.Ordinal);

        if (index1 != -1 && index2 != -1)
        {
            var setters = contentString.Remove(index1, index2 - index1 + 2);
            Setters = parser.TryGetSetters(setters);
            
            index1 += 2;
            var childSection = contentString[index1..index2];
            parser    = parser.Clone(childSection);
            Resources = parser.TryGetResources();
            Children  = parser.TryGetStyles();
        }
        else
        {
            Setters   = parser.TryGetSetters(contentString);
            Resources = Enumerable.Empty<CssResourceList>();
            Children  = Enumerable.Empty<CssStyle>();
        }
    }

    internal Style ToAvaloniaStyle(bool isChild)
    {
        this.WriteLine($"==== Begin parsing style with raw selector of '{Selector}'.");

        // Selector
        var selector   = isChild ? Selectors.Nesting(null) : null;
        var style      = new Style();
        var syntaxList = SelectorGrammar.Parse(Selector).ToList();
        var selectors  = new List<Selector>();

        foreach (var syntax in syntaxList)
        {
            if (syntax is CommaSyntax)
            {
                if (selector != null)
                {
                    selectors.Add(selector);
                }
                selector = isChild ? Selectors.Nesting(null) : null;
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
            // Resources
            foreach (var cssResourceList in Resources)
            {
                cssResourceList.TryAddTo(style.Resources.MergedDictionaries);
            }

            // Setters
            foreach (var setter in Setters.Select(s => s.ToAvaloniaSetter(style.Selector.TargetType)).OfType<ISetter>())
            {
                style.Add(setter);
            }

            // Children
            foreach (var cssStyle in Children)
            {
                var childStyle = cssStyle.ToAvaloniaStyle(true);
                style.Add(childStyle);
            }
        }

        return style;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"'{Selector}'");
        foreach (var cssSetter in Setters)
        {
            builder.AppendLine($"    {cssSetter.ToString()}");
        }
        return builder.ToString();
    }
}
