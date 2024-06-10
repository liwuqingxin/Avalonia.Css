using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class FlexLayout : MagicLayout
{
    public static FlexLayout Default { get; } = new();
    
    private FlexLayout()
    {
        
    }
    
    public override IEnumerable<string> GetNames()
    {
        yield return "Flex";
        yield return "FlexPanel";
    }

    public override Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize)
    {
        var orientation = MagicPanel.GetOrientation(panel);
        var maca        = orientation.GetMaCa();
        
        // If the main axis has no restriction, just regard as stack layout.
        if (double.IsInfinity(maca.MaV(availableSize)))
        {
            return StackLayout.Default.MeasureOverride(panel, children, availableSize);
        }
        
        // Measure all children.
        children.JustMeasure(availableSize);
        
        // Deal with wrap.
        var flexWrap = MagicPanel.GetFlexWrap(panel);
        return flexWrap switch
        {
            FlexWrap.NoWrap      => MeasureNoWrap(panel, children, availableSize, maca),
            FlexWrap.Wrap        => MeasureWrap(panel, children, availableSize, maca),
            FlexWrap.WrapReverse => MeasureWrapReverse(panel, children, availableSize, maca),
            _                    => new Size()
        };
    }

    public override IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }



    #region Measures
    
    private Size MeasureNoWrap(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize, IMaCa maca)
    {
        var spacing        = MagicPanel.GetSpacing(panel);
        var alignItems     = MagicPanel.GetAlignItems(panel);
        var justifyContent = MagicPanel.GetJustifyContent(panel);
        var alignContent   = MagicPanel.GetAlignContent(panel);
        
        var panelDesiredWidth  = 0d;
        var panelDesiredHeight = 0d;

        var childrenDesired = children.Sum(child => maca.MaV(child.DesiredSize));
        
        // Constraint cross axis.
        var constraintSize = availableSize;
        constraintSize.ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(children, maca);

        foreach (var child in children)
        {
            继续
            var desiredSize = child.DesiredSize;
            
            // Align items to the point start.
            var childAlignment = MagicPanel.GetAlignment(child);
            var start = LayoutHelper.LocateStartWithAlignment(
                alignItems, 
                childAlignment,
                maca.CaV(constraintSize),
                maca.CaV(desiredSize),
                out var isStretch);
            
            // Tile and align the child.
            child.TileAndAlign(panelDesiredWidth, panelDesiredHeight, start, maca);
            
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

    private Size MeasureWrap(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize, IMaCa maca)
    {
        throw new NotImplementedException();
    }

    private Size MeasureWrapReverse(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize, IMaCa maca)
    {
        throw new NotImplementedException();
    }

    #endregion
}