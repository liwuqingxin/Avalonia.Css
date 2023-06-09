﻿using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Thickness))]
[ResourceType("StrokeThickness")]
[ResourceType("Margin")]
[ResourceType("Padding")]
public class ThicknessResource : CssResource<ThicknessResource>
{
    protected override object? Accept(string valueString)
    {
        try
        {
            return Thickness.Parse(valueString);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
