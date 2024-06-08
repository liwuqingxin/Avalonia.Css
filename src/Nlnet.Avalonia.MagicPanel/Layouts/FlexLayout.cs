using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class FlexLayout : MagicLayout
{
    private struct FlexInfo
    {
        public FlexInfo(
            Size           constraintSize,
            bool           isHorizontal,
            double         spacing,
            Alignment      alignItems,
            JustifyContent justifyContent,
            AlignContent   alignContent)
        {
            ConstraintSize = constraintSize;
            IsHorizontal   = isHorizontal;
            Spacing        = spacing;
            AlignItems     = alignItems;
            JustifyContent = justifyContent;
            AlignContent   = alignContent;
        }

        public Size ConstraintSize { get; }

        public bool IsHorizontal { get; }

        public double Spacing { get; }

        public Alignment AlignItems { get; }

        public JustifyContent JustifyContent { get; }

        public AlignContent AlignContent { get; }
    }

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
        var orientation  = MagicPanel.GetOrientation(panel);
        var isHorizontal = orientation == Orientation.Horizontal;
        
        // If the main axis has no restriction, just regard as stack layout.
        if (isHorizontal)
        {
            if (double.IsInfinity(availableSize.Width))
            {
                return StackLayout.Default.MeasureOverride(panel, children, availableSize);
            }
        }
        else
        {
            if (double.IsInfinity(availableSize.Height))
            {
                return StackLayout.Default.MeasureOverride(panel, children, availableSize);
            }
        }
        
        // Properties.
        var spacing        = MagicPanel.GetSpacing(panel);
        var alignment      = MagicPanel.GetAlignItems(panel);
        var justifyContent = MagicPanel.GetJustifyContent(panel);
        var alignContent   = MagicPanel.GetAlignContent(panel);
        var flexWrap       = MagicPanel.GetFlexWrap(panel);
        
        // All info.
        var info = new FlexInfo(availableSize, isHorizontal, spacing, alignment, justifyContent, alignContent);

        // Measure all children.
        children.JustMeasure(availableSize);
        
        // Deal with wrap.
        return flexWrap switch
        {
            FlexWrap.NoWrap      => MeasureNoWrap(panel, children, info),
            FlexWrap.Wrap        => MeasureWrap(panel, children, info),
            FlexWrap.WrapReverse => MeasureWrapReverse(panel, children, info),
            _                    => new Size()
        };
    }

    public override IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }



    #region Measures
    
    private Size MeasureNoWrap(MagicPanel panel, IReadOnlyList<Control> children, FlexInfo info)
    {
        return new Size();
        
        // var panelDesiredWidth  = 0d;
        // var panelDesiredHeight = 0d;
        // var index              = 0;
        //
        // // Measure all children.
        // var constraintSize = availableSize;
        // children.JustMeasure(constraintSize, out var existedVisible);
        //
        // // Constraint all.
        // constraintSize.ConstraintCrossDirectionWithChildrenMaxDesiredIfNotConstraint(children, isHorizontal);
        //
        // for (var count = children.Count; index < count; ++index)
        // {
        //     var child       = children[index];
        //     var desiredSize = child.DesiredSize;
        //     
        //     // Location
        //     var isStretch      = false;
        //     var childAlignment = MagicPanel.GetAlignment(child); 
        //     if (isHorizontal)
        //     {
        //         var start = LayoutHelper.LocateStartWithAlignment(alignment, childAlignment, constraintSize.Height, desiredSize.Height, out isStretch);
        //         Canvas.SetLeft(child, panelDesiredWidth);
        //         Canvas.SetTop(child, start);
        //         
        //         panelDesiredWidth  = panelDesiredWidth + spacing + desiredSize.Width;
        //         panelDesiredHeight = Math.Max(panelDesiredHeight, desiredSize.Height);
        //     }
        //     else
        //     {
        //         var start = LayoutHelper.LocateStartWithAlignment(alignment, childAlignment, constraintSize.Width, desiredSize.Width, out isStretch);
        //         Canvas.SetLeft(child, start);
        //         Canvas.SetTop(child, panelDesiredHeight);
        //         
        //         panelDesiredWidth  = Math.Max(panelDesiredWidth, desiredSize.Width);
        //         panelDesiredHeight = panelDesiredHeight + spacing + desiredSize.Height;
        //     }
        //     
        //     // Size
        //     var width  = child.DesiredSize.Width;
        //     var height = child.DesiredSize.Height;
        //     if (isStretch)
        //     {
        //         // TODO Test for availableSize.
        //         if (isHorizontal)
        //         {
        //             height = constraintSize.Height;
        //         }
        //         else
        //         {
        //             width = constraintSize.Width;
        //         }    
        //     }
        //     
        //     LayoutEx.SetArrangedWidth(child, width);
        //     LayoutEx.SetArrangedHeight(child, height);
        // }
        //
        // var size = isHorizontal
        //     ? new Size(panelDesiredWidth - (existedVisible ? spacing : 0.0), panelDesiredHeight)
        //     : new Size(panelDesiredWidth, panelDesiredHeight - (existedVisible ? spacing : 0.0));
        //
        // return size;
    }

    private Size MeasureWrap(MagicPanel panel, IReadOnlyList<Control> children, FlexInfo info)
    {
        throw new NotImplementedException();
    }

    private Size MeasureWrapReverse(MagicPanel panel, IReadOnlyList<Control> children, FlexInfo info)
    {
        throw new NotImplementedException();
    }

    #endregion



    #region No Wrap

    private static double ArrangeMainDirection(MagicPanel panel, IReadOnlyList<Control> children, FlexInfo info)
    {
        
        var totalDesired = children.Sum(c => c.DesiredSize.Width);

        return 0;
    }
    
    private static double ArrangeCrossDirection(MagicPanel panel, IReadOnlyList<Control> children, FlexInfo info)
    {
        // Constraint all.
        var constraintSize = info.ConstraintSize;
        // constraintSize.ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(children, info.IsHorizontal);

        foreach (var child in children)
        {
            var start = LayoutHelper.LocateStartWithAlignment(
                info.AlignItems, 
                MagicPanel.GetAlignment(child),
                constraintSize.Height,
                child.DesiredSize.Height,
                out var isStretch);    
        }
        
        
        switch (info.AlignItems)
        {
            case Alignment.Stretch:
            {
                
                return constraintSize.Height;
            }
            case Alignment.Center:
                break;
            case Alignment.Start:
                break;
            case Alignment.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return 0;
    }

    #endregion

    private static List<double> CalculatePosition(IList<Control> children, Size constraintSize, bool isHorizontal)
    {
        var points = new List<double>();
        if (isHorizontal)
        {
            var totalDesired = children.Sum(c => c.DesiredSize.Width);
            if (totalDesired >= constraintSize.Width)
            {
                var cursor = 0d;
                foreach (var child in children)
                {
                    points.Add(cursor);
                    cursor += child.DesiredSize.Width / totalDesired * constraintSize.Width;
                }
            }
            else
            {
                
            }
        }
        else
        {
            
        }

        return points;
    }
}