using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(BoxShadows))]
[ResourceType(nameof(BoxShadow))]
public class BoxShadowsResource : CssResource<BoxShadowsResource>
{
    protected override object? Accept(string valueString)
    {
        try
        {
            return BoxShadows.Parse(valueString);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
