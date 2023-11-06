using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NotSyntax : ISyntax
{
    public IEnumerable<ISyntax> Argument { get; set; } = Enumerable.Empty<ISyntax>();

    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        var manager = context.GetService<ITypeResolverManager>();
        var interpreter = context.GetService<IAcssInterpreter>();

        var selector = interpreter.ToSelector(context, acssStyle, Argument);
        if (selector == null)
        {
            this.WriteError($"Can not apply ':not' selector for {Argument}");
            return previous;
        }

        return previous?.Not(selector);
    }
}
