using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class IsSyntax : ISyntax, ITypeSyntax
{
    public string TypeName { get; set; } = string.Empty;

    public string Xmlns { get; set; } = string.Empty;

    public Selector? ToSelector(ICssBuilder builder, Selector? previous)
    {
        var manager = builder.TypeResolver;
        if (manager.TryGetType(TypeName, out var type))
        {
            return previous.Is(type!);
        }

        return previous;
    }
}
