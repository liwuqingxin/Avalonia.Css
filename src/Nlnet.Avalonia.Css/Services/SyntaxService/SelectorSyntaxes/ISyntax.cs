using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface ISyntax
{
    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous);
}
