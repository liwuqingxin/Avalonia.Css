using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class DescendantSyntax : ISyntax
{
    public Selector? ToSelector(IAcssBuilder builder, IAcssStyle acssStyle, Selector? previous)
    {
        return previous?.Descendant();
    }
}
