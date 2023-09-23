using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class DescendantSyntax : ISyntax
{
    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        return previous?.Descendant();
    }
}
