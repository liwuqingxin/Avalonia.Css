using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

internal interface ICssSectionFactory
{
    public ICssSection Build(ICssParser parser, ICssSection? parent, string selector, ReadOnlySpan<char> content);
}

internal class CssSectionFactory : ICssSectionFactory
{
    private readonly Regex _regexResource  = new(":res\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexAnimation = new(":animation\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexThemeChildStyle = new("^\\s*>\\s*(.*)", RegexOptions.IgnoreCase);

    public ICssSection Build(ICssParser parser, ICssSection? parent, string selector, ReadOnlySpan<char> content)
    {
        ICssSection section;
        if (_regexResource.IsMatch(selector))
        {
            section = new CssResourceDictionary(selector);
        }
        else if (_regexAnimation.IsMatch(selector))
        {
            section = new CssAnimation(selector);
        }
        else if (_regexThemeChildStyle.Match(selector) is { Success: true } match)
        {
            section = new CssStyle(match.Groups[1].Value)
            {
                IsThemeChild = true,
            };
        }
        else
        {
            section = new CssStyle(selector);
        }

        section.Parent = parent;
        section.InitialSection(parser, content);
        return section;
    }
}
