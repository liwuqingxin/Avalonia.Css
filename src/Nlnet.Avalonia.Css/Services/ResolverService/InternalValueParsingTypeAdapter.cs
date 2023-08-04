using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Internal value parsing type adapter.
/// </summary>
internal class InternalValueParsingTypeAdapter : ValueParsingTypeAdapter
{
    public InternalValueParsingTypeAdapter()
    {
        // Brush.
        AddAdaptType(typeof(IBrush), typeof(Brush));
        AddAdaptType(typeof(ISolidColorBrush), typeof(Brush));
        AddAdaptType(typeof(SolidColorBrush), typeof(Brush));
        
        // Transform.
        AddAdaptType(typeof(ITransform), typeof(TransformOperations));
        AddAdaptType(typeof(Transform), typeof(TransformOperations));
        
        // Internal Parsers.
        AddAdaptType(typeof(Transitions), typeof(TransitionsParser));
    }
}
