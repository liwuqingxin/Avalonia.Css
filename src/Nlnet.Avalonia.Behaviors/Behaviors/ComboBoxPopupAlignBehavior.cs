using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Behaviors;

[Behavior("combobox.popup.align", typeof(ComboBox))]
public class ComboBoxPopupAlignBehavior : AcssBehavior
{
    private double _horizontalOffset;
    private double _verticalOffset;
    private bool   _isAttached;

    public override void OnAttached(AvaloniaObject target)
    {
        if (target is not ComboBox comboBox)
        {
            return;
        }

        if (comboBox.IsLoaded)
        {
            AttachToComboBox();
        }
        else
        {
            comboBox.Loaded += OnAssociatedObjectOnLoaded;
        }
    }

    public override void OnDetached(AvaloniaObject target)
    {
        if (target is not ComboBox comboBox)
        {
            return;
        }

        var popup = comboBox.FindDescendantOfType<Popup>();
        if (popup == null)
        {
            return;
        }

        _isAttached = false;

        popup.Opened -= Popup_Opened;
    }

    private void Popup_Opened(object? sender, EventArgs e)
    {
        var comboBox  = AssociatedObject!;
        var popup     = (Popup)sender!;
        var popupRoot = popup.Child!;

        var index = comboBox.SelectedIndex;
        if (index == -1 && comboBox.ItemCount > 0)
        {
            index = 0;
        }
        if (index == -1)
        {
            return;
        }

        var container = comboBox.ContainerFromIndex(index);
        var matrix    = container?.TransformToVisual(popupRoot);
        if (matrix == null)
        {
            return;
        }

        var point = new Point().Transform(matrix.Value);

        var toOffset   = -(comboBox.Bounds.Height + point.Y) + _verticalOffset;
        var fromOffset = toOffset;
        //if (popup.VerticalOffset > toOffset)
        //{
        //    fromOffset += 4;
        //}
        //else if(popup.VerticalOffset < toOffset)
        //{
        //    fromOffset -= 4;
        //}
        var transitions = popup.Transitions;
        popup.Transitions      = null;
        popup.VerticalOffset   = fromOffset;
        popup.Transitions      = transitions;
        popup.VerticalOffset   = toOffset;
        popup.HorizontalOffset = /*-point.X*/ +_horizontalOffset;
    }
}
