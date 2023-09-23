using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class NestingSyntax : ISyntax
{
    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        throw new NotImplementedException("This selector should not be used in acss.");
        // return previous.Nesting();
    }
}