using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(BoxShadows))]
[ResourceType(nameof(BoxShadow))]
[ResourceType("shadow")]
internal class BoxShadowsResource : AcssResourceBaseAndFac<BoxShadowsResource>
{
    protected override object? BuildValue(IAcssBuilder acssBuilder, string valueString)
    {
        return valueString.TryParseBoxShadow();
    }
}
