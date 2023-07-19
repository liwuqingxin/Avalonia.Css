using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NotSyntax : ISyntax
{
    public IEnumerable<ISyntax> Argument { get; set; } = Enumerable.Empty<ISyntax>();

    public Selector? ToSelector(ICssBuilder builder, ICssStyle cssStyle, Selector? previous)
    {
        var selector = builder.Interpreter.ToSelector(builder, cssStyle, Argument);
        if (selector == null)
        {
            this.WriteLine($"Can not apply :not selector for {Argument}");
            return previous;
        }

        return previous?.Not(selector);
    }
}
