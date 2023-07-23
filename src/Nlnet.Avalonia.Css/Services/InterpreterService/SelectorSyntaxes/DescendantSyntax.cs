using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class DescendantSyntax : ISyntax
{
    public Selector? ToSelector(ICssBuilder builder, ICssStyle cssStyle, Selector? previous)
    {
        return previous?.Descendant();
    }
}
