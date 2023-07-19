using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface ISyntax
{
    public Selector? ToSelector(ICssBuilder builder, ICssStyle cssStyle, Selector? previous);
}
