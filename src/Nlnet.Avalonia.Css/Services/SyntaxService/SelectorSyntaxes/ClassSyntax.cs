using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class ClassSyntax : ISyntax
{
    public string Class { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        return previous.Class(Class);
    }
}
