﻿using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class NotSyntax : ISyntax
{
    public IEnumerable<ISyntax> Argument { get; set; } = Enumerable.Empty<ISyntax>();

    public Selector? ToSelector(Selector? previous)
    {
        var selector = InterpreterHelper.ToSelector(Argument);
        if (selector == null)
        {
            this.WriteLine($"Can not apply :not selector for {Argument}");
            return previous;
        }

        return previous?.Not(selector);
    }
}
