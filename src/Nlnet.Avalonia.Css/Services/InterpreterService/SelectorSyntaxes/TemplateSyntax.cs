using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class TemplateSyntax : ISyntax
{
    public Selector? ToSelector(ICssBuilder builder, Selector? previous)
    {
        return previous?.Template();
    }
}
