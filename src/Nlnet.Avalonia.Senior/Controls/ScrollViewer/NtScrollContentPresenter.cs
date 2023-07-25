using System;
using System.Collections;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Nlnet.Avalonia.Senior.Controls;

/// <summary>
/// <see cref="ScrollContentPresenter"/> to scroll smoothly.
/// </summary>
/// TODO ScrollGesture, ChildChanged, UpdateScrollableSubscription, UpdateFromScrollable
public class NtScrollContentPresenter : ScrollContentPresenter
{
    public Vector AnimatableOffset
    {
        get { return GetValue(AnimatableOffsetProperty); }
        private set { SetValue(AnimatableOffsetProperty, value); }
    }
    public static readonly StyledProperty<Vector> AnimatableOffsetProperty = AvaloniaProperty
        .Register<NtScrollContentPresenter, Vector>(nameof(AnimatableOffset));



    static NtScrollContentPresenter()
    {
        var isUpdating = false;

        AnimatableOffsetProperty.Changed.AddClassHandler<NtScrollContentPresenter>((presenter, args) =>
        {
            if (isUpdating)
            {
                return;
            }
            // Update offset of scroll viewer. Animating value should not be passed to scroll viewer.
            //var localValue = presenter.GetBaseValue(AnimatableOffsetProperty, BindingPriority.LocalValue);
            var localValue = presenter.AnimatableOffset;
            if (presenter.TemplatedParent is NtScrollViewer viewer)
            {
                //viewer.Offset = localValue.Value;
                //viewer.Offset = localValue;
            }

            // Update offset of presenter.
            isUpdating = true;
            presenter.SetCurrentValue(OffsetProperty, presenter.AnimatableOffset);
            isUpdating = false;
            //presenter.Offset = presenter.AnimatableOffset;
        });

        ScrollViewer.OffsetProperty.Changed.AddClassHandler<NtScrollViewer>((viewer, args) =>
        {
            if (isUpdating)
            {
                return;
            }
            if (viewer.Presenter is not NtScrollContentPresenter presenter)
            {
                return;
            }

            //var localValue = viewer.GetBaseValue(ScrollViewer.OffsetProperty, BindingPriority.LocalValue);
            //presenter.AnimatableOffset = localValue.Value;

            // Update AnimatableOffset when parent ScrollViewer's offset is changed.
            var localValue = viewer.Offset;
            isUpdating = true;
            presenter.AnimatableOffset = localValue;
            isUpdating = false;
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {

        base.OnPropertyChanged(change);
    }

    public NtScrollContentPresenter() : base()
    {
        // Remove the event handler for RequestBringIntoViewEvent that registered in the base ctor.
        TryRemoveRequestBringIntoViewEventHandler();

        // We will override it.
        AddHandler(RequestBringIntoViewEvent, BringIntoViewRequested);
    }

    private void TryRemoveRequestBringIntoViewEventHandler()
    { 
        // TODO Check it when upgrade Avalonia.

        var eventHandlersFiled =
            typeof(Interactive).GetField("_eventHandlers", BindingFlags.NonPublic | BindingFlags.Instance);

        var dic = eventHandlersFiled?.GetValue(this) as IDictionary;
        dic?.Remove(RequestBringIntoViewEvent);
    }

    private void BringIntoViewRequested(object? sender, RequestBringIntoViewEventArgs e)
    {
        if (e.TargetObject is not null)
        {
            e.Handled = BringDescendantIntoView(e.TargetObject, e.TargetRect);
        }
    }

    /// <summary>
    /// New method for <see cref="ScrollContentPresenter.BringDescendantIntoView"/>.
    /// Attempts to bring a portion of the target visual into view by scrolling the content.
    /// </summary>
    /// <param name="target">The target visual.</param>
    /// <param name="targetRect">The portion of the target visual to bring into view.</param>
    /// <returns>True if the scroll offset was changed; otherwise false.</returns>
    private new bool BringDescendantIntoView(Visual target, Rect targetRect)
    {
        if (Child?.IsEffectivelyVisible != true)
        {
            return false;
        }

        var scrollable = Child as ILogicalScrollable;
        if (scrollable?.IsLogicalScrollEnabled == true && target is Control control)
        {
            return scrollable.BringIntoView(control, targetRect);
        }

        var transform = target.TransformToVisual(Child);

        if (transform == null)
        {
            return false;
        }

        var rect   = targetRect.TransformToAABB(transform.Value);
        var offset = Offset;
        var result = false;

        if (Offset.X + Viewport.Width > Child.Bounds.Width)
        {
            offset = offset.WithX(Child.Bounds.Width - Viewport.Width);
            result = true;
        }
        if (Offset.Y + Viewport.Height > Child.Bounds.Height)
        {
            offset = offset.WithY(Child.Bounds.Height - Viewport.Height);
            result = true;
        }

        if (rect.Bottom > offset.Y + Viewport.Height)
        {
            offset = offset.WithY((rect.Bottom - Viewport.Height) + Child.Margin.Top);
            result = true;
        }

        if (rect.Y < offset.Y)
        {
            offset = offset.WithY(rect.Y);
            result = true;
        }

        if (rect.Right > offset.X + Viewport.Width)
        {
            offset = offset.WithX((rect.Right - Viewport.Width) + Child.Margin.Left);
            result = true;
        }

        if (rect.X < offset.X)
        {
            offset = offset.WithX(rect.X);
            result = true;
        }

        if (result)
        {
            AnimatableOffset = offset;
        }

        return result;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (this.TemplatedParent is not NtScrollViewer viewer)
        {
            base.OnPointerWheelChanged(e);
            return;
        }

        if (Extent.Height > Viewport.Height || Extent.Width > Viewport.Width)
        {
            var scrollable = Child as ILogicalScrollable;
            var isLogical  = scrollable?.IsLogicalScrollEnabled == true;

            var x = Offset.X;
            var y = Offset.Y;

            if (Extent.Height > Viewport.Height)
            {
                var height = isLogical ? scrollable!.ScrollSize.Height : viewer.SmoothScrollingStep;
                y += -e.Delta.Y * height;
                y =  Math.Max(y, 0);
                y =  Math.Min(y, Extent.Height - Viewport.Height);
            }

            if (Extent.Width > Viewport.Width)
            {
                var width = isLogical ? scrollable!.ScrollSize.Width : viewer.SmoothScrollingStep;
                x += -e.Delta.X * width;
                x =  Math.Max(x, 0);
                x =  Math.Min(x, Extent.Width - Viewport.Width);
            }

            var newOffset     = new Vector(x, y);
            var offsetChanged = newOffset != Offset;

            // Do not set it directly to keep template binding available.
            AnimatableOffset = newOffset;
            //viewer.Offset = newOffset;

            e.Handled = !IsScrollChainingEnabled || offsetChanged;
        }
    }
}