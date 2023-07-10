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
    private readonly ICssBuilder _builder;

    private readonly Regex _regexResource        = new(":res\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexAnimation       = new(":animation\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexThemeChildStyle = new("^\\s*>\\s*(.*)", RegexOptions.IgnoreCase);

    public CssSectionFactory(ICssBuilder builder)
    {
        _builder = builder;
    }

    public ICssSection Build(ICssParser parser, ICssSection? parent, string selector, ReadOnlySpan<char> content)
    {
        ICssSection section;
        if (_regexResource.IsMatch(selector))
        {
            section = new CssResourceDictionary(_builder, selector);
        }
        else if (_regexAnimation.IsMatch(selector))
        {
            section = new CssAnimation(_builder, selector);
        }
        else if (_regexThemeChildStyle.Match(selector) is { Success: true } match)
        {
            section = new CssStyle(_builder, match.Groups[1].Value)
            {
                IsThemeChild = true,
            };
        }
        else
        {
            section = new CssStyle(_builder, selector);
        }

        section.Parent = parent;
        section.InitialSection(parser, content);
        return section;
    }
}
