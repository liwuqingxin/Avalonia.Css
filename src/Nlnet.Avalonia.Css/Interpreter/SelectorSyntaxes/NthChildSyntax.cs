using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NthChildSyntax : ISyntax
{
    public int Offset { get; set; }
    public int Step   { get; set; }

    public Selector? ToSelector(Selector? previous)
    {
        return previous?.NthChild(Step, Offset);
    }
}
