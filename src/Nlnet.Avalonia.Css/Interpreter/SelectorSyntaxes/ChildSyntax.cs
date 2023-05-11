using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class ChildSyntax : ISyntax
{
    public Selector? ToSelector(Selector? previous)
    {
        return previous?.Child();
    }
}
