using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface ISyntax
{
    public Selector? ToSelector(IAcssBuilder builder, IAcssStyle acssStyle, Selector? previous);
}
