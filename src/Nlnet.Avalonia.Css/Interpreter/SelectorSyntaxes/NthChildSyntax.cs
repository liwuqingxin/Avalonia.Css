using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class NthChildSyntax : ISyntax
{
    public int Offset { get; set; }
    public int Step   { get; set; }

    public Selector? ToSelector(Selector? previous)
    {
        return previous?.NthChild(Step, Offset);
    }
}
