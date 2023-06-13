using System;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css;

internal static class CssSectionFactory
{
    private static readonly Regex RegexResource  = new(":res(\\[.*\\])?", RegexOptions.IgnoreCase);
    private static readonly Regex RegexAnimation = new(":animation(\\[.*\\])?", RegexOptions.IgnoreCase);

    public static ICssSection Build(ICssParser parser, string selector, ReadOnlySpan<char> content)
    {
        ICssSection section;
        if (RegexResource.IsMatch(selector))
        {
            section = new CssResourceDictionary(selector);
        }
        else if (RegexAnimation.IsMatch(selector))
        {
            section = new CssAnimation(selector);
        }
        else
        {
            section = new CssStyle(selector);
        }

        section.InitialSection(parser, content);
        return section;
    }
}
