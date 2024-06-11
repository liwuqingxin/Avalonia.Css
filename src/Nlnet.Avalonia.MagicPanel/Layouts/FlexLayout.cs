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

        var totalSpacing                = spacing * (children.Count - 1);
        var childrenDesired             = children.Sum(child => child.GetFlexBasis(maca));
        var spaced                      = maca.MaV(availableSize) - childrenDesired - totalSpacing;
        var computedSizes               = ComputeChildrenSize(children, spaced, maca);
        var totalComputedSize           = computedSizes.Aggregate(0d, (i, d) => i + d);
        var justifyContentSpace         = maca.MaV(availableSize) - totalComputedSize - totalSpacing;
        var justifyContentPieces        = justifyContent.GetJustifyContentSpacePieces(children.Count);
        var justifyContentSpacePerPiece = justifyContentSpace / justifyContentPieces;
        var justifyContentStart         = justifyContent.GetJustifyContentStart(justifyContentSpacePerPiece);
        var justifyContentSpaceBetween  = justifyContent.GetJustifyContentSpaceBetween(justifyContentSpacePerPiece);
        
        // Constraint cross axis.
        var constraintSize = availableSize;
        constraintSize.ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(children, maca);

        for (var i = 0; i < children.Count; i++)
        {
            var child        = children[i];
            var computedSize = computedSizes[i];
            var desiredSize  = child.DesiredSize;

            // Align items to the point start.
            var childAlignment = MagicPanel.GetAlignSelf(child);
            var start = LayoutHelper.LocateStartWithAlignment(
                alignItems,
                childAlignment,
                maca.CaV(constraintSize),
                maca.CaV(desiredSize),
                out var isStretch);
            maca.Align(child, start);
            
            计算


            // Calculate panel desired size.
            // maca.AccumulateMav(ref panelDesiredWidth, ref panelDesiredHeight, spacing + maca.MaV(desiredSize));
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

    private static IList<double> ComputeChildrenSize(IReadOnlyList<Control> children, double spaced, IMaCa maca)
    {
        switch (spaced)
        {
            case 0:
            {
                return children.Select(c => c.GetFlexBasis(maca)).ToList();
            }
            case > 0:
            {
                var pieces = children.Sum(MagicPanel.GetFlexGrow);
                if (pieces == 0)
                {
                    return children.Select(c => c.GetFlexBasis(maca)).ToList();
                }

                var spacePerPiece = spaced / Math.Max(pieces, 1);
                return children.Select(c => c.GetFlexBasis(maca) + MagicPanel.GetFlexGrow(c) * spacePerPiece).ToList();
            }
            case < 0:
            {
                var pieces = children.Sum(MagicPanel.GetFlexShrink);
                if (pieces == 0)
                {
                    return children.Select(c => c.GetFlexBasis(maca)).ToList();
                }

                var spacePerPiece = spaced / Math.Max(pieces, 1);
                return children.Select(c => c.GetFlexBasis(maca) + MagicPanel.GetFlexShrink(c) * spacePerPiece).ToList();
            }
        }
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