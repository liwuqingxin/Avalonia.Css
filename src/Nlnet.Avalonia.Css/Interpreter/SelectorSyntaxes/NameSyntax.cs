using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class NameSyntax : ISyntax
{
    public string Name { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        return previous.Name(Name);
    }
}
