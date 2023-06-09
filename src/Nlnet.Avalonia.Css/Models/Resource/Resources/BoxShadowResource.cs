using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

public class BoxShadowResource : CssResource<BoxShadowResource>
{
    protected override object? Accept(string valueString)
    {
        try
        {
            return BoxShadow.Parse(valueString);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
