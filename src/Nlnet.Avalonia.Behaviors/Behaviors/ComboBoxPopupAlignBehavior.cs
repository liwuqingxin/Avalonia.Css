using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Nlnet.Avalonia.Behaviors;

public interface IBehaviorFactory
{
    public AcssBehavior GetAcssBehavior();
}

public class FactoryComboBoxPopupAlignBehavior : IBehaviorFactory
{
    public AcssBehavior GetAcssBehavior()
    {
        return new ComboBoxPopupAlignBehavior();
    }
}

public static class AcssBehaviorFactories
{
    static AcssBehaviorFactories()
    {
        var factories = typeof(AcssBehaviorFactories).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(AcssBehavior)) && t.IsAbstract == false)
            .Select(t => (attr: t.GetCustomAttribute<BehaviorAttribute>(), facType: t))
            .Where(tuple => tuple.attr != null)
            .Select(tuple => (tuple.attr, fac: Activator.CreateInstance(tuple.facType) as ISvgTagFactory))
            .ToDictionary(tuple => tuple.attr!.Tag, tuple => tuple.fac)!;


        typeof(AcssBehaviorFactories).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(AcssBehavior)) && t.IsAbstract == false)
    }
}

[Behavior("combobox.popup.align", typeof(ComboBox))]
public class ComboBoxPopupAlignBehavior : AcssBehavior
{
    private double _horizontalOffset;
    private double _verticalOffset;
    private bool   _isAttached;

    protected override void OnAttached(AvaloniaObject target)
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
            comboBox.Loaded += AssociatedObjectOnLoaded;
        }
    }

    private void AssociatedObjectOnLoaded(object? sender, RoutedEventArgs args)
    {
        if (AssociatedObject is not ComboBox comboBox)
        {
            return;
        }

        comboBox.Loaded -= AssociatedObjectOnLoaded;

        AttachToComboBox();
    }

    private void AttachToComboBox()
    {
        if (AssociatedObject is not ComboBox comboBox)
        {
            return;
        }

        var popup = Get<ComboBox>().FindDescendantOfType<Popup>();

        if (popup == null || _isAttached)
        {
            return;
        }

        if (_horizontalOffset == 0)
        {
            _horizontalOffset = popup.HorizontalOffset;
        }
        if (_verticalOffset == 0)
        {
            _verticalOffset = popup.VerticalOffset;
        }

        _isAttached = true;

        popup.Opened += Popup_Opened;
    }

    private void Popup_Opened(object? sender, EventArgs e)
    {
        if (AssociatedObject is not ComboBox comboBox)
        {
            return;
        }

        var popup     = (Popup)sender!;
        var popupRoot = popup.Child!;
        var index     = comboBox.SelectedIndex;
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

    protected override void OnDetached(AvaloniaObject target)
    {
        var popup = Get<ComboBox>().FindDescendantOfType<Popup>();
        if (popup == null)
        {
            return;
        }

        _isAttached = false;

        popup.Opened -= Popup_Opened;
    }
}
