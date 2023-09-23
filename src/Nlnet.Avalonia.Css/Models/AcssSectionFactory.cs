using System;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css;

internal interface IAcssSectionFactory : IService
{
    public IAcssSection Build(IAcssParser parser, AcssTokens tokens, IAcssSection? parent, string selector, ReadOnlySpan<char> content);
}

internal class AcssSectionFactory : IAcssSectionFactory
{
    private readonly IAcssContext _context;

    private readonly Regex _regexResource          = new("^\\s*::res\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexAnimation         = new("^\\s*::animation\\s*(\\[.*\\])?", RegexOptions.IgnoreCase);
    private readonly Regex _regexThemeChildStyle   = new("^\\s*\\^\\s*(.*)", RegexOptions.IgnoreCase);
    private readonly Regex _regexLogicalChildStyle = new("^\\s*>\\s*(.*)", RegexOptions.IgnoreCase);

    public AcssSectionFactory(IAcssContext context)
    {
        _context = context;
    }

    public void Initialize()
    {

    }

    public IAcssSection Build(IAcssParser parser, AcssTokens tokens, IAcssSection? parent, string header, ReadOnlySpan<char> content)
    {
        IAcssSection section;
        if (_regexResource.IsMatch(header))
        {
            section = new AcssResourceDictionary(_context, header);
        }
        else if (_regexAnimation.IsMatch(header))
        {
            section = new AcssAnimation(_context, header);
        }
        else if (_regexThemeChildStyle.Match(header) is { Success: true } match1)
        {
            section = new AcssStyle(_context, tokens, header.Trim()[1..])
            {
                IsThemeChild = true,
            };
        }
        else if (_regexLogicalChildStyle.Match(header) is { Success: true } match2)
        {
            section = new AcssStyle(_context, tokens, header.Trim()[1..])
            {
                IsLogicalChild = true,
            };
        }
        else
        {
            section = new AcssStyle(_context, tokens, header);
        }

        section.Parent = parent;
        section.InitialSection(parser, content);
        return section;
    }
}
