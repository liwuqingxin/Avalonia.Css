using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface ISyntax
{
    public Selector? ToSelector(Selector? previous);
}
