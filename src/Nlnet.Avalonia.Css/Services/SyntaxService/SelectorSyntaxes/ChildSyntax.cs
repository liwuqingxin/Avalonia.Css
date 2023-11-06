using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class ChildSyntax : ISyntax
{
    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        return previous?.Child();
    }
}
