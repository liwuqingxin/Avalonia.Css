using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NameSyntax : ISyntax
{
    public string Name { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        return previous.Name(Name);
    }
}
