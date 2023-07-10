using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NthLastChildSyntax : ISyntax
{
    public int Offset { get; set; }
    public int Step   { get; set; }

    public Selector? ToSelector(Selector? previous)
    {
        return previous?.NthLastChild(Step, Offset);
    }
}
