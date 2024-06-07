using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Nlnet.Avalonia.Controls;
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Nlnet.Avalonia;

public abstract class MagicLayout : AvaloniaObject, IMagicLayout
{
    #region IMagicLayout

    public abstract IEnumerable<string> GetNames();

    public abstract Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize);

    public virtual Size ArrangeOverride(MagicPanel panel, IReadOnlyList<Control> children, Size finalSize)
    {
        foreach (var child in children)
        {
            if (child.IsEffectivelyVisible == false)
            {
                continue;
            }
            
            var location = child.GetTopLeft(finalSize);
            var width    = LayoutEx.GetArrangedWidth(child);
            var height   = LayoutEx.GetArrangedHeight(child);
            
            if (double.IsFinite(location.X) == false)
            {
                Trace.WriteLine("MagicPanel: location of X is not finite.");
                location = location.WithX(0);
            }
            if (double.IsFinite(location.Y) == false)
            {
                Trace.WriteLine("MagicPanel: location of Y is not finite.");
                location = location.WithY(0);
            }
            if (double.IsFinite(width) == false)
            {
                Trace.WriteLine("MagicPanel: width is not finite.");
                width = finalSize.Width;
            }
            if (double.IsFinite(height) == false)
            {
                Trace.WriteLine("MagicPanel: height is not finite.");
                height = finalSize.Height;
            }
        
            child.Arrange(new Rect(location, new Size(width, height)));
        }

        return finalSize;
    }

    public abstract IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap);

    public virtual void ApplySetter(MagicPanel panel, string property, string value)
    {
        // css: https://www.runoob.com/w3cnote/flex-grammar.html
        
        switch (property)
        {
            case "space":
            case "spacing":
            case "gap":
            {
                panel.ApplySpacing(value);
                break;
            }
            case "align":
            case "alignment":
            case "alignitems":
            case "align-items":
            {
                panel.ApplyAlignItems(value);
                break;
            }
            case "justify":
            case "justifycontent":
            case "justify-content":
            {
                panel.ApplyJustifyContent(value);
                break;
            }
            case "align-content":
            {
                panel.ApplyAlignContent(value);
                break;
            }
            case "orientation":
            case "direction":
            case "flex-direction":
            {
                panel.ApplyOrientation(value);
                break;
            }
            case "r":
            case "reverse":
            {
                panel.ApplyReverse(value);
                break;
            }
            case "wrap":
            case "flex-wrap":
            {
                panel.ApplyFlexWrap(value);
                break;
            }
            case "flex-flow":
            {
                // flex-flow: <flex-direction> <flex-wrap>;
                var values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2)
                {
                    break;
                }
                
                ApplySetter(panel, "flex-direction", values[0]);
                ApplySetter(panel, "flex-wrap",      values[1]);
                break;
            }
        }
    }

    #endregion
}