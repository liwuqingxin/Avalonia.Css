using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Transformation;

namespace Nlnet.Avalonia.Senior.Controls
{
    public class RecyclePanel : Panel
    {
        private double _offset;

        private double _extendHeight;
        private double _centerTop;
        private double _centerBottom;
        private int    _halfCount;
        private int    _realizedCount;
        private int    _containerCountOffset;
        private int    _selectedVirtualIndex    = -1;
        private bool   _suspressSelectedChanged = false;



        #region Avalonia Properties

        public IEnumerable? ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty
            .Register<RecyclePanel, IEnumerable?>(nameof(ItemsSource));

        public double ItemHeight
        {
            get { return GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
        public static readonly StyledProperty<double> ItemHeightProperty = AvaloniaProperty
            .Register<RecyclePanel, double>(nameof(ItemHeight), 28d);

        private double SelectedItemHeight
        {
            get { return GetValue(SelectedItemHeightProperty); }
            set { SetValue(SelectedItemHeightProperty, value); }
        }
        private static readonly StyledProperty<double> SelectedItemHeightProperty = AvaloniaProperty
            .Register<RecyclePanel, double>(nameof(SelectedItemHeight), 28d);

        public static int GetItemIndex(Visual host)
        {
            return host.GetValue(ItemIndexProperty);
        }
        public static void SetItemIndex(Visual host, int value)
        {
            host.SetValue(ItemIndexProperty, value);
        }
        public static readonly AttachedProperty<int> ItemIndexProperty = AvaloniaProperty
            .RegisterAttached<RecyclePanel, Visual, int>("ItemIndex");

        public object? SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty
            .Register<RecyclePanel, object?>(nameof(SelectedItem), null, false, BindingMode.TwoWay, null, CoerceSelectedItem, true);

        private static object? CoerceSelectedItem(AvaloniaObject arg1, object? arg2)
        {
            return arg2 ?? (arg1 as RecyclePanel)?.SelectedItem;
        }

        #endregion



        #region ctor

        static RecyclePanel()
        {
            AffectsMeasure<RecyclePanel>(ItemHeightProperty, SelectedItemHeightProperty);
            AffectsArrange<RecyclePanel>(ItemIndexProperty);

            ItemsSourceProperty.Changed.AddClassHandler<RecyclePanel>(OnItemsSourceChanged);
            SelectedItemProperty.Changed.AddClassHandler<RecyclePanel>(OnSelectedItemChanged);
        }


        public RecyclePanel()
        {
            var transitions = new Transitions()
            {
                new TransformOperationsTransition()
                {
                    Duration = new TimeSpan(0, 0, 0, 0, 300),
                    Property = RenderTransformProperty,
                },
            };

            this.SetCurrentValue(TransitionsProperty, transitions);
        }

        #endregion



        #region Feature

        private static void OnItemsSourceChanged(RecyclePanel panel, AvaloniaPropertyChangedEventArgs arg)
        {
            panel.CalculateVirtualization();
            panel.RealizeContainers();
        }

        private static void OnSelectedItemChanged(RecyclePanel panel, AvaloniaPropertyChangedEventArgs arg)
        {
            if (panel._suspressSelectedChanged == true || panel.ItemsSource == null || arg.NewValue == null || arg.OldValue == null)
            {
                return;
            }

            var oldItem  = arg.OldValue;
            var newItem  = arg.NewValue;
            var items    = panel.ItemsSource.OfType<object>().ToList();
            var indexOld = items.IndexOf(oldItem);
            var indexNew = items.IndexOf(newItem);

            if (indexNew == indexOld)
            {
                return;
            }
            
            var countOffset = indexOld - indexNew;
            panel.ScrollCountAndUpdateContainers(countOffset);
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            this.CalculateVirtualization();
            this.RealizeContainers();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var visualChildren = this.VisualChildren;
            var count = visualChildren.Count;
            for (var index = 0; index < count; ++index)
            {
                if (visualChildren[index] is Layoutable layoutable)
                {
                    layoutable.Measure(availableSize);
                }
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = GetSortedChildren();
            var count    = children.Count;
            var cursor   = -_containerCountOffset * ItemHeight;
            for (var index = 0; index < count; ++index)
            {
                if (children[index] is Layoutable layoutable)
                {
                    layoutable.Arrange(new Rect(0, cursor, finalSize.Width, ItemHeight));
                }
                cursor += ItemHeight;
            }

            return finalSize;
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            switch (e.Delta.Y)
            {
                case > 0:
                    LineDown();
                    break;
                case < 0:
                    LineUp();
                    break;
            }

            e.Handled = true;
        }

        #endregion



        #region Helper

        private static int GetIndexForItems(int virtualIndex, int itemCount)
        {
            virtualIndex %= itemCount;
            virtualIndex += itemCount;
            virtualIndex %= itemCount;
            
            return virtualIndex;
        }

        private IList<Visual> GetSortedChildren()
        {
            var visualChildren = this.VisualChildren.ToList();
            visualChildren.Sort((v1, v2) =>
            {
                var i1 = GetItemIndex(v1);
                var i2 = GetItemIndex(v2);
                return i1 - i2;
            });
            return visualChildren;
        }

        #endregion



        #region Initial

        private void CalculateVirtualization()
        {
            _extendHeight  = Bounds.Height;
            _centerTop     = (_extendHeight - SelectedItemHeight) / 2;
            _centerBottom  = _centerTop + SelectedItemHeight;
            _halfCount     = (int)Math.Ceiling(_centerTop / ItemHeight);
            _realizedCount = _halfCount * 4 + 1;
            _offset        = -((ItemHeight - (_centerTop % ItemHeight)) % ItemHeight + _halfCount * ItemHeight);

            ScrollTo(_offset);
        }

        private void RealizeContainers()
        {
            this.Children.Clear();

            if (ItemsSource == null)
            {
                return;
            }
            var items = ItemsSource.OfType<object>().ToList();

            for (var i = -_halfCount * 2; i < _realizedCount - _halfCount * 2; i++)
            {
                var container = new ListBoxItem();
                container.PointerPressed += ContainerOnPointerPressed;

                SetItemIndex(container, i);
                FillContainer(container, items);
                this.Children.Add(container);
            }

            UpdateSelection();
        }

        private void ContainerOnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is not ListBoxItem container)
            {
                return;
            }

            var index       = GetItemIndex(container);
            var offsetCount = _selectedVirtualIndex - index;
            ScrollCountAndUpdateContainers(offsetCount);
        }

        #endregion



        #region Selection

        private void UpdateSelection()
        {
            var children = GetSortedChildren();
            var count    = children.Count;
            var center   = count / 2;

            _selectedVirtualIndex = GetItemIndex(children[center]);

            for (var i = 0; i < count; i++)
            {
                if (children[i] is ListBoxItem listBoxItem)
                {
                    var selected = i == center;
                    if (selected)
                    {
                        listBoxItem.IsSelected   = true;
                        _suspressSelectedChanged = true;
                        SetCurrentValue(SelectedItemProperty, listBoxItem.Content);
                        _suspressSelectedChanged = false;
                    }
                    else
                    {
                        listBoxItem.IsSelected = false;
                    }
                }
            }
        }

        #endregion



        #region Scroll

        public void LineUp()
        {
            ScrollCountAndUpdateContainers(-1);
        }

        public void LineDown()
        {
            ScrollCountAndUpdateContainers(1);
        }

        private void ScrollCountAndUpdateContainers(int countOffset)
        {
            RecycleContainers(countOffset);
            InvalidateArrange();

            _offset += countOffset * ItemHeight;
            ScrollTo(_offset);
        }

        private void ScrollTo(double offset)
        {
            var translate = TransformOperations.Parse($"translate(0, {offset}px)");
            this.RenderTransform = translate;
        }

        #endregion



        #region Realize

        private void RecycleContainers(int countOffset)
        {
            _containerCountOffset += countOffset;
            if (countOffset == 0 || VisualChildren.Count == 0 || ItemsSource == null)
            {
                return;
            }

            var items = ItemsSource.OfType<object>().ToList();

            if (countOffset > 0)
            {
                // Scroll up.
                for (var i = 0; i < countOffset; i++)
                {
                    var children     = GetSortedChildren();
                    var first        = children.First();
                    var last         = children.Last();
                    var virtualIndex = GetItemIndex(first) - 1;

                    SetItemIndex(last, virtualIndex);

                    FillContainer(last, items);
                }
            }
            else
            {
                // Scroll down.
                for (var i = 0; i < Math.Abs(countOffset); i++)
                {
                    var children     = GetSortedChildren();
                    var first        = children.First();
                    var last         = children.Last();
                    var virtualIndex = GetItemIndex(last) + 1;

                    SetItemIndex(first, virtualIndex);

                    FillContainer(first, items);
                }
            }

            UpdateSelection();
        }

        private static void FillContainer(Visual container, IList<object> items)
        {
            var virtualIndex = GetItemIndex(container);
            var itemIndex    = GetIndexForItems(virtualIndex, items.Count);
            if (container is ListBoxItem listBoxItem)
            {
                listBoxItem.Content = items[itemIndex];
            }
        }

        #endregion
    }
}
