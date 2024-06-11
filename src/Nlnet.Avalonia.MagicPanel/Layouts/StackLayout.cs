using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class StackLayout : MagicLayout
{
    public static StackLayout Default { get; } = new();

    private StackLayout()
    {
        
    }
    
    public override IEnumerable<string> GetNames()
    {
        yield return "Stack";
        yield return "StackPanel";
    }

    public override Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize)
    {
        var orientation = MagicPanel.GetOrientation(panel);
        var spacing     = MagicPanel.GetSpacing(panel);
        var alignment   = MagicPanel.GetAlignItems(panel);
        var maca        = orientation.GetMaCa();

        var panelDesiredWidth  = 0d;
        var panelDesiredHeight = 0d;
        
        // Measure all children.
        var constraintSize = availableSize;
        constraintSize.LiberateMainAxis(maca);
        children.JustMeasure(constraintSize);

        // Constraint cross axis.
        constraintSize.ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(children, maca);

        foreach (var child in children)
        {
            var desiredSize = child.DesiredSize;
            
            // Align items to the point start.
            var childAlignment = MagicPanel.GetAlignSelf(child);
            var start = LayoutHelper.LocateStartWithAlignment(
                alignment, 
                childAlignment,
                maca.CaV(constraintSize),
                maca.CaV(desiredSize),
                out var isStretch);
            
            // Tile and align the child.
            maca.Tile(child, maca.MaV(panelDesiredWidth, panelDesiredHeight));
            maca.Align(child, start);
            
            // Calculate panel desired size.
            maca.AccumulateMav(ref panelDesiredWidth, ref panelDesiredHeight, spacing + maca.MaV(desiredSize));
            maca.MaxCav(ref panelDesiredWidth, ref panelDesiredHeight, maca.CaV(desiredSize));
            
            // Size.
            var width  = child.DesiredSize.Width;
            var height = child.DesiredSize.Height;
            if (isStretch)
            {
                maca.WithCav(ref width, ref height, maca.CaV(constraintSize));
            }
            LayoutEx.SetArrangedWidth(child, width);
            LayoutEx.SetArrangedHeight(child, height);
        }
        
        // Remove last spacing.
        maca.AccumulateMav(ref panelDesiredWidth, ref panelDesiredHeight, -spacing);
        
        return new Size(panelDesiredWidth, panelDesiredHeight);
    }

    public override IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }
}