﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public interface ICssStyle : ICssSection
{
    public IEnumerable<ICssSetter>? Setters { get; set; }

    public IEnumerable<ICssStyle>? Styles { get; set; }

    public IEnumerable<ICssResourceDictionary>? Resources { get; set; }

    public IEnumerable<ICssAnimation>? Animations { get; set; }

    public Style ToAvaloniaStyle();
}

public class CssStyle : CssSection, ICssStyle
{
    public IEnumerable<ICssSetter>? Setters { get; set; }

    public IEnumerable<ICssStyle>? Styles { get; set; }

    public IEnumerable<ICssResourceDictionary>? Resources { get; set; }

    public IEnumerable<ICssAnimation>? Animations { get; set; }

    public CssStyle(string selector) : base(selector)
    {

    }

    public override void InitialSection(ICssParser parser, ReadOnlySpan<char> content)
    {
        parser.ParseSettersAndChildren(content, out var settersSpan, out var childrenSpan);

        Setters = parser.ParsePairs(settersSpan).Select(p => new CssSetter(p.Item1, p.Item2));
        var list = parser.ParseSections(childrenSpan).ToList();
        if (list.Count > 0)
        {
            foreach (var section in list)
            {
                section.Parent = this;
            }

            Children   = list;
            Styles     = list.OfType<ICssStyle>();
            Resources  = list.OfType<ICssResourceDictionary>();
            Animations = list.OfType<ICssAnimation>();
        }
    }

    public Style ToAvaloniaStyle()
    {
        this.WriteLine($"==== Begin parsing style with raw selector of '{Selector}'.");

        var isChild = Parent != null;

        // Selector
        var selector = isChild ? Selectors.Nesting(null) : null;
        var style = new Style();
        var syntaxList = SelectorGrammar.Parse(Selector).ToList();
        var selectors = new List<Selector>();

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
            if (Resources != null)
            {
                foreach (var cssResourceList in Resources)
                {
                    var dic = cssResourceList.ToAvaloniaResourceDictionary();
                    if (dic != null)
                    {
                        style.Resources.MergedDictionaries.Add((dic));
                    }
                }
            }

            // Setters
            if (Setters != null)
            {
                foreach (var setter in Setters.Select(s => s.ToAvaloniaSetter(style.Selector.TargetType)).OfType<ISetter>())
                {
                    style.Add(setter);
                }
            }

            // Children Styles
            if (Styles != null)
            {
                foreach (var cssStyle in Styles)
                {
                    var childStyle = cssStyle.ToAvaloniaStyle();
                    style.Add(childStyle);
                }
            }

            // Style Animations
            if (Animations != null)
            {
                foreach (var cssAnimation in Animations)
                {
                    var animation = cssAnimation.ToAvaloniaAnimation();
                    if (animation != null)
                    {
                        style.Animations.Add(animation);
                    }
                }
            }
        }

        return style;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"'{Selector}'");
        if (Setters != null)
        {
            foreach (var cssSetter in Setters)
            {
                builder.AppendLine($"    {cssSetter.ToString()}");
            }
        }
        return builder.ToString();
    }
}
