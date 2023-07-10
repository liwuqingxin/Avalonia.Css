using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class ChildSyntax : ISyntax
{
    public Selector? ToSelector(Selector? previous)
    {
        return previous?.Child();
    }
}
