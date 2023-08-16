using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class ClassSyntax : ISyntax
{
    public string Class { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssBuilder builder, IAcssStyle acssStyle, Selector? previous)
    {
        return previous.Class(Class);
    }
}
