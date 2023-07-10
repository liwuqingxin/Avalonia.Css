using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class DescendantSyntax : ISyntax
{
    public Selector? ToSelector(Selector? previous)
    {
        return previous?.Descendant();
    }
}
