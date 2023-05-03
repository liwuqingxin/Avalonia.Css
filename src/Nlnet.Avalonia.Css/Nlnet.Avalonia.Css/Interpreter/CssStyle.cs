using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class CssStyle
{
    private string Selector { get; set; }

    private IEnumerable<CssSetter> Setters { get; set; }

    public CssStyle(ICssParser parser, string selector, string setters)
    {
        Selector = selector.Trim();
        Setters  = parser.TryGetSetters(setters);
    }

    public IStyle ToAvaloniaStyle()
    {
        this.WriteLine($"==== Begin parsing style with raw selector of '{Selector}'.");

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
        builder.AppendLine($"'{Selector}'");
        foreach (var cssSetter in Setters)
        {
            builder.AppendLine($"    {cssSetter.ToString()}");
        }
        return builder.ToString();
    }
}
