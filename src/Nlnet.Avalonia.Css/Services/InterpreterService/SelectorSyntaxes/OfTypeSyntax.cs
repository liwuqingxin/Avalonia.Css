using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class OfTypeSyntax : ISyntax, ITypeSyntax
{
    public string TypeName { get; set; } = string.Empty;

    public string Xmlns { get; set; } = string.Empty;

    public Selector? ToSelector(ICssBuilder builder, Selector? previous)
    {
        var manager = builder.TypeResolver;
        if (manager.TryGetType(TypeName, out var type))
        {
            return previous.OfType(type!);
        }

        this.WriteLine($"Can not resolve type '{TypeName}'");

        return previous;
    }
}
