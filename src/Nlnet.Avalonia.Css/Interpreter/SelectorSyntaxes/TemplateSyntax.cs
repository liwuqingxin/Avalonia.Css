using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class TemplateSyntax : ISyntax
{
    public Selector? ToSelector(Selector? previous)
    {
        return previous?.Template();
    }
}
