using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NameSyntax : ISyntax
{
    public string Name { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssBuilder builder, IAcssStyle acssStyle, Selector? previous)
    {
        return previous.Name(Name);
    }
}
