using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class TemplateSyntax : ISyntax
{
    public Selector? ToSelector(Selector? previous)
    {
        return previous?.Template();
    }
}
