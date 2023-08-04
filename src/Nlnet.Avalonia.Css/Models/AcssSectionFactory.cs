using System;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css;

internal interface IAcssSectionFactory
{
    public IAcssSection Build(IAcssParser parser, IAcssSection? parent, string selector, ReadOnlySpan<char> content);
}

internal class AcssSectionFactory : IAcssSectionFactory
{
    private readonly IAcssBuilder _builder;

    private readonly Regex _regexResource          = new("^\\s*::res\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexAnimation         = new("^\\s*::animation\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexThemeChildStyle   = new("^\\s*\\^\\s*(.*)", RegexOptions.IgnoreCase);
    private readonly Regex _regexLogicalChildStyle = new("^\\s*>\\s*(.*)", RegexOptions.IgnoreCase);

    public AcssSectionFactory(IAcssBuilder builder)
    {
        _builder = builder;
    }

    public IAcssSection Build(IAcssParser parser, IAcssSection? parent, string selector, ReadOnlySpan<char> content)
    {
        IAcssSection section;
        if (_regexResource.IsMatch(selector))
        {
            section = new AcssResourceDictionary(_builder, selector);
        }
        else if (_regexAnimation.IsMatch(selector))
        {
            section = new AcssAnimation(_builder, selector);
        }
        else if (_regexThemeChildStyle.Match(selector) is { Success: true } match1)
        {
            section = new AcssStyle(_builder, match1.Groups[1].Value)
            {
                IsThemeChild = true,
            };
        }
        else if (_regexLogicalChildStyle.Match(selector) is { Success: true } match2)
        {
            section = new AcssStyle(_builder, match2.Groups[1].Value)
            {
                IsLogicalChild = true,
            };
        }
        else
        {
            section = new AcssStyle(_builder, selector);
        }

        section.Parent = parent;
        section.InitialSection(parser, content);
        return section;
    }
}
