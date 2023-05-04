﻿using System;
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

    public CssStyle(ICssParser parser, string selector, string content)
    {
        Selector = selector.Trim();

        var index1 = content.IndexOf("[[", StringComparison.Ordinal);
        var index2 = content.IndexOf("]]", StringComparison.Ordinal);

        if (index1 != -1 && index2 != -1)
        {
            var setters = content.Remove(index1, index2 - index1 + 2);
            index1 += 2;
            var childrenStyles = content[index1..index2];
            Setters  = parser.TryGetSetters(setters);
            Children = parser.TryGetStyles(childrenStyles);
        }
        else
        {
            Setters  = parser.TryGetSetters(content);
            Children = Enumerable.Empty<CssStyle>();
        }
    }

    public Style ToAvaloniaStyle()
    {
        this.WriteLine($"==== Begin parsing style with raw selector of '{Selector}'.");

        // Selector
        Selector? selector   = null;

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
            // Setters
            foreach (var setter in Setters.Select(s => s.ToAvaloniaSetter(style.Selector.TargetType)).OfType<ISetter>())
            {
                style.Add(setter);
            }

            // Children
            foreach (var cssStyle in Children)
            {
                var childStyle = cssStyle.ToAvaloniaStyle();
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
