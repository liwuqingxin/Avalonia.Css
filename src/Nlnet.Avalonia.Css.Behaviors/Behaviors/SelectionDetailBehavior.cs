using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;

namespace Nlnet.Avalonia.Css.Behaviors;

[Behavior("selection.detail", typeof(Acss))]
public class SelectionDetailBehavior :  AcssBehavior<SelectionDetailBehavior>
{
    static SelectionDetailBehavior()
    {
        SelectingItemsControl.SelectedIndexProperty.Changed.AddClassHandler<SelectingItemsControl>(OnSelectedIndexChanged);
    }

    private static void OnSelectedIndexChanged(SelectingItemsControl itemsControl, AvaloniaPropertyChangedEventArgs arg)
    {
        var newValue = arg.GetNewValue<int>();
        var oldValue = arg.GetOldValue<int>();
    }

    protected override void OnAttached(AvaloniaObject target)
    {
        if (target is not SelectingItemsControl itemsControl)
        {
            return;
        }

        
        itemsControl.GetObservable(SelectingItemsControl.SelectedIndexProperty).Subscribe(x => { });
        itemsControl.SelectionChanged -= ItemsControlOnSelectionChanged;
        itemsControl.SelectionChanged += ItemsControlOnSelectionChanged;
    }

    private void Action(SelectingItemsControl arg1, AvaloniaPropertyChangedEventArgs arg2)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDetached(AvaloniaObject target)
    {
        if (target is not SelectingItemsControl itemsControl)
        {
            return;
        }
        
        itemsControl.SelectionChanged -= ItemsControlOnSelectionChanged;
    }
    
    
    private void ItemsControlOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        e.
    }
}