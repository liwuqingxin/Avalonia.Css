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
        
        // Deal with wrapping.
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
        return MeasureLine(panel, children, availableSize, maca, 0);
    }

    private Size MeasureWrap(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize, IMaCa maca)
    {
        var alignContent   = MagicPanel.GetAlignContent(panel);


        return new Size();
    }

    private Size MeasureWrapReverse(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize, IMaCa maca)
    {
        return new Size();
    }

    #endregion



    private static Size MeasureLine(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize, IMaCa maca, double lineStart)
    {
        var gap            = MagicPanel.GetSpacing(panel);
        var alignItems     = MagicPanel.GetAlignItems(panel);
        var justifyContent = MagicPanel.GetJustifyContent(panel);

        var totalGap          = gap * (children.Count - 1);
        var childrenDesired   = children.Sum(child => child.GetFlexBasis(maca));
        var spaced            = maca.MaV(availableSize) - childrenDesired - totalGap;
        var computedSizes     = children.ComputeSizes(spaced, maca);
        var totalComputedSize = computedSizes.Aggregate(0d, (i, d) => i + d);
        var jcSpace           = maca.MaV(availableSize) - totalComputedSize - totalGap;
        var jcPieces          = justifyContent.GetJustifyContentSpacePieces(children.Count);
        var jcSpacePerPiece   = jcSpace / jcPieces;
        var jcStart           = justifyContent.GetJustifyContentStart(jcSpacePerPiece);
        var jcSpaceBetween    = justifyContent.GetJustifyContentSpaceBetween(jcSpacePerPiece);
        
        var panelDesiredWidth  = 0d;
        var panelDesiredHeight = 0d;
        
        // Set start of the main axis.
        maca.WithMav(ref panelDesiredWidth, ref panelDesiredHeight, jcStart);
        
        // Constraint cross axis.
        var constraintSize = availableSize;
        constraintSize.ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(children, maca);

        for (var i = 0; i < children.Count; i++)
        {
            var child        = children[i];
            var computedSize = computedSizes[i];

            // Tile and align the child.
            maca.Tile(child, maca.MaV(panelDesiredWidth, panelDesiredHeight));
            
            // Align items to the point start.
            var childAlignment = MagicPanel.GetAlignSelf(child);
            var start = LayoutHelper.LocateStartWithAlignment(
                alignItems,
                childAlignment,
                maca.CaV(constraintSize),
                maca.CaV(child.DesiredSize),
                out var isStretch) + lineStart;
            maca.Align(child, start);

            // Size.
            var width   = 0d;
            var height  = 0d;
            var cavSize = maca.CaV(child.DesiredSize);
            if (isStretch)
            {
                cavSize = maca.CaV(constraintSize);
            }
            maca.WithMav(ref width, ref height, computedSize);
            maca.WithCav(ref width, ref height, cavSize);
            LayoutEx.SetArrangedWidth(child, width);
            LayoutEx.SetArrangedHeight(child, height);
            
            // Calculate panel desired size.
            maca.AccumulateMav(ref panelDesiredWidth, ref panelDesiredHeight, gap + jcSpaceBetween + maca.MaV(width, height));
            maca.MaxCav(ref panelDesiredWidth, ref panelDesiredHeight, maca.CaV(width, height));
        }

        // Remove last spacing.
        maca.AccumulateMav(ref panelDesiredWidth, ref panelDesiredHeight, -gap);
        
        return new Size(panelDesiredWidth, panelDesiredHeight);
    }
}