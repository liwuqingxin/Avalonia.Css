using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class IsSyntax : ISyntax, ITypeSyntax
{
    public string TypeName { get; set; } = string.Empty;

    public string Xmlns { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        var manager = ServiceLocator.GetService<ITypeResolverManager>();
        if (manager.TryGetType(TypeName, out var type))
        {
            return previous.Is(type!);
        }

        return previous;
    }
}
