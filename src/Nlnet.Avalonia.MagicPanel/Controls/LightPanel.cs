using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Controls;

[Obsolete]
public class LightPanel : Control, IChildIndexProvider
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty = 
        Border.BackgroundProperty.AddOwner<LightPanel>();

    private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;

    static LightPanel() => AffectsRender<LightPanel>(BackgroundProperty);

    public LightPanel()
    {
        Children.CollectionChanged += ChildrenChanged;
    }

    [Content] public global::Avalonia.Controls.Controls Children { get; } = new();

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public bool IsItemsHost { get; internal set; }

    event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
    {
        add
        {
            if (_childIndexChanged == null)
            {
                Children.PropertyChanged += ChildrenPropertyChanged;
            }

            _childIndexChanged += value;
        }
        remove
        {
            _childIndexChanged -= value;
            if (_childIndexChanged != null)
            {
                return;
            }

            Children.PropertyChanged -= ChildrenPropertyChanged;
        }
    }

    public sealed override void Render(DrawingContext context)
    {
        var background = Background;
        if (background != null)
        {
            var size = Bounds.Size;
            context.FillRectangle(background, new Rect(size));
        }

        base.Render(context);
    }

    protected static void AffectsParentArrange<TPanel>(params AvaloniaProperty[] properties)
        where TPanel : LightPanel
    {
        var anonymousObserver =
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(AffectsParentArrangeInvalidate<TPanel>);
        foreach (var property in properties)
        {
            property.Changed.Subscribe(anonymousObserver);
        }
    }

    protected static void AffectsParentMeasure<TPanel>(params AvaloniaProperty[] properties)
        where TPanel : LightPanel
    {
        var anonymousObserver =
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(AffectsParentMeasureInvalidate<TPanel>);
        foreach (var property in properties)
        {
            property.Changed.Subscribe(anonymousObserver);
        }
    }

    protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (!IsItemsHost)
                {
                    LogicalChildren.InsertRange(e.NewStartingIndex,
                        e.NewItems.OfType<Control>().ToList<Control>());
                }

                VisualChildren.InsertRange(e.NewStartingIndex, e.NewItems.OfType<Visual>());
                break;
            case NotifyCollectionChangedAction.Remove:
                if (!IsItemsHost)
                {
                    LogicalChildren.RemoveAll(
                        e.OldItems.OfType<Control>().ToList<Control>());
                }

                VisualChildren.RemoveAll(e.OldItems.OfType<Visual>());
                break;
            case NotifyCollectionChangedAction.Replace:
                for (var index1 = 0; index1 < e.OldItems.Count; ++index1)
                {
                    var index2  = index1 + e.OldStartingIndex;
                    var newItem = (Control)e.NewItems[index1];
                    if (!IsItemsHost)
                    {
                        LogicalChildren[index2] = newItem;
                    }

                    VisualChildren[index2] = newItem;
                }

                break;
            case NotifyCollectionChangedAction.Move:
                if (!IsItemsHost)
                {
                    LogicalChildren.MoveRange(e.OldStartingIndex, e.OldItems.Count, e.NewStartingIndex);
                }

                VisualChildren.MoveRange(e.OldStartingIndex, e.OldItems.Count, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }

        var childIndexChanged = _childIndexChanged;
        childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);

        InvalidateMeasureOnChildrenChanged();
    }

    private protected virtual void InvalidateMeasureOnChildrenChanged() => InvalidateMeasure();

    private void ChildrenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != "Count" && e.PropertyName != null)
        {
            return;
        }

        var childIndexChanged = _childIndexChanged;
        childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.TotalCountChanged);
    }

    private static void AffectsParentArrangeInvalidate<TPanel>(AvaloniaPropertyChangedEventArgs e)
        where TPanel : LightPanel
    {
        ((e.Sender is Control sender ? sender.GetVisualParent() : null) as TPanel)?.InvalidateArrange();
    }

    private static void AffectsParentMeasureInvalidate<TPanel>(AvaloniaPropertyChangedEventArgs e)
        where TPanel : LightPanel
    {
        ((e.Sender is Control sender ? sender.GetVisualParent() : null) as TPanel)?.InvalidateMeasure();
    }

    int IChildIndexProvider.GetChildIndex(ILogical child)
    {
        return !(child is Control control) ? -1 : Children.IndexOf(control);
    }

    bool IChildIndexProvider.TryGetTotalCount(out int count)
    {
        count = Children.Count;
        return true;
    }
}