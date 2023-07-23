using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class ClassSyntax : ISyntax
{
    public string Class { get; set; } = string.Empty;

    public Selector? ToSelector(ICssBuilder builder, ICssStyle cssStyle, Selector? previous)
    {
        return previous.Class(Class);
    }
}
