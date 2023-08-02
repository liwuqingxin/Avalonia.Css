using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(BoxShadows))]
[ResourceType(nameof(BoxShadow))]
internal class BoxShadowsResource : CssResourceBaseAndFac<BoxShadowsResource>
{
    protected override object? Accept(IAcssBuilder cssBuilder, string valueString)
    {
        try
        {
            return BoxShadows.Parse(valueString);
        }
        catch
        {
            this.WriteError($"Can not parse {nameof(BoxShadows)} from string '{valueString}'.");
            return null;
        }
    }
}
