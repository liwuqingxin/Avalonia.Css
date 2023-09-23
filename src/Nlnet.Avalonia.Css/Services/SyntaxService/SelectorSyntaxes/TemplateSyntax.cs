using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class TemplateSyntax : ISyntax
{
    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        return previous?.Template();
    }
}
