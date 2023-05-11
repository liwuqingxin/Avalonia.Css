using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class IsSyntax : ISyntax, ITypeSyntax
{
    public string TypeName { get; set; } = string.Empty;

    public string Xmlns { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        if (TypeResolver.Instance.TryGetType(TypeName, out var type))
        {
            return previous.Is(type!);
        }

        return previous;
    }
}
