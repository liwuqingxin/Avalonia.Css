using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class ClassSyntax : ISyntax
{
    public string Class { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        return previous.Class(Class);
    }
}
