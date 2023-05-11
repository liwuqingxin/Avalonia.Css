using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public interface ISyntax
{
    public Selector? ToSelector(Selector? previous);
}
