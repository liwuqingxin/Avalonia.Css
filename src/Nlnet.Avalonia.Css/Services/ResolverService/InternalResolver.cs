using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Internal type resolver.
/// </summary>
internal class InternalTypeResolver : Resolver, ITypeResolver
{
    public InternalTypeResolver()
    {
        TryAddType(nameof(TextDecorations), typeof(TextDecorations));
        //TryAddType(nameof(RhythmicTransformAnimator), typeof(RhythmicTransformAnimator));
    }
}
