using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class DescendantSyntax : ISyntax
{
    public Selector? ToSelector(Selector? previous)
    {
        return previous?.Descendant();
    }
}
