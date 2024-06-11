using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Nlnet.Avalonia.Controls
{
    public sealed class MagicPanel : Panel, INavigableContainer /*, IScrollSnapPointsInfo*/
    {
        private IMagicLayout _layout = StackLayout.Default;


        
        #region Panel Properties

        // Spacing
        public static double GetSpacing(MagicPanel host)
        {
            return host.GetValue(SpacingProperty);
        }
        public static void SetSpacing(MagicPanel host, double value)
        {
            host.SetValue(SpacingProperty, value);
        }
        public static readonly AttachedProperty<double> SpacingProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, double>("Spacing", 0d);

        // AlignItems
        public static Alignment GetAlignItems(MagicPanel host)
        {
            return host.GetValue(AlignItemsProperty);
        }
        public static void SetAlignItems(MagicPanel host, Alignment value)
        {
            host.SetValue(AlignItemsProperty, value);
        }
        public static readonly AttachedProperty<Alignment> AlignItemsProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, Alignment>("AlignItems", Alignment.Stretch);

        // JustifyContent
        public static JustifyContent GetJustifyContent(MagicPanel host)
        {
            return host.GetValue(JustifyContentProperty);
        }
        public static void SetJustifyContent(MagicPanel host, JustifyContent value)
        {
            host.SetValue(JustifyContentProperty, value);
        }
        public static readonly AttachedProperty<JustifyContent> JustifyContentProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, JustifyContent>("JustifyContent", JustifyContent.Center);

        // AlignContent
        public static AlignContent GetAlignContent(MagicPanel host)
        {
            return host.GetValue(AlignContentProperty);
        }
        public static void SetAlignContent(MagicPanel host, AlignContent value)
        {
            host.SetValue(AlignContentProperty, value);
        }
        public static readonly AttachedProperty<AlignContent> AlignContentProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, AlignContent>("AlignContent", AlignContent.Stretch);

        // Orientation
        public static Orientation GetOrientation(MagicPanel host)
        {
            return host.GetValue(OrientationProperty);
        }
        public static void SetOrientation(MagicPanel host, Orientation value)
        {
            host.SetValue(OrientationProperty, value);
        }
        public static readonly AttachedProperty<Orientation> OrientationProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, Orientation>("Orientation", Orientation.Vertical);

        // Reverse
        public static bool GetReverse(MagicPanel host)
        {
            return host.GetValue(ReverseProperty);
        }
        public static void SetReverse(MagicPanel host, bool value)
        {
            host.SetValue(ReverseProperty, value);
        }
        public static readonly AttachedProperty<bool> ReverseProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, bool>("Reverse", false);

        // FlexWrap
        public static FlexWrap GetFlexWrap(MagicPanel host)
        {
            return host.GetValue(FlexWrapProperty);
        }
        public static void SetFlexWrap(MagicPanel host, FlexWrap value)
        {
            host.SetValue(FlexWrapProperty, value);
        }
        public static readonly AttachedProperty<FlexWrap> FlexWrapProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, MagicPanel, FlexWrap>("FlexWrap", FlexWrap.NoWrap);

        #endregion



        #region Item Properties
        
        // AlignSelf
        public static Alignment? GetAlignSelf(Control host)
        {
            return host.GetValue(AlignSelfProperty);
        }
        public static void SetAlignSelf(Control host, Alignment? value)
        {
            host.SetValue(AlignSelfProperty, value);
        }
        public static readonly AttachedProperty<Alignment?> AlignSelfProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, Control, Alignment?>("AlignSelf");

        // Order
        public static int GetOrder(Control host)
        {
            return host.GetValue(OrderProperty);
        }
        public static void SetOrder(Control host, int value)
        {
            host.SetValue(OrderProperty, value);
        }
        public static readonly AttachedProperty<int> OrderProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, Control, int>("Order");

        // FlexGrow
        public static double GetFlexGrow(Control host)
        {
            return host.GetValue(FlexGrowProperty);
        }
        public static void SetFlexGrow(Control host, double value)
        {
            host.SetValue(FlexGrowProperty, value);
        }
        public static readonly AttachedProperty<double> FlexGrowProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, Control, double>("FlexGrow", 0);

        // FlexShrink
        public static double GetFlexShrink(Control host)
        {
            return host.GetValue(FlexShrinkProperty);
        }
        public static void SetFlexShrink(Control host, double value)
        {
            host.SetValue(FlexShrinkProperty, value);
        }
        public static readonly AttachedProperty<double> FlexShrinkProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, Control, double>("FlexShrink", 1);

        // FlexBasis
        public static double GetFlexBasis(Control host)
        {
            return host.GetValue(FlexBasisProperty);
        }
        public static void SetFlexBasis(Control host, double value)
        {
            host.SetValue(FlexBasisProperty, value);
        }
        public static readonly AttachedProperty<double> FlexBasisProperty = AvaloniaProperty
            .RegisterAttached<MagicPanel, Control, double>("FlexBasis", double.NaN);

        #endregion


        
        #region Layouts

        private static readonly Dictionary<string, IMagicLayout> InternalLayouts = new(StringComparer.OrdinalIgnoreCase);

        public static IReadOnlyDictionary<string, IMagicLayout> Layouts => InternalLayouts;

        public static void RegisterLayout(IMagicLayout layout)
        {
            var names = layout.GetNames();
            foreach (var name in names)
            {
                if (InternalLayouts.TryGetValue(name, out _))
                {
                    throw new InvalidOperationException($"Layout definition with same name of '{name}' has existed.");
                }

                InternalLayouts.Add(name, layout);
            }
        }

        #endregion


        
        #region Properties

        public string? Layout
        {
            get => GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }
        public static readonly StyledProperty<string?> LayoutProperty = AvaloniaProperty
            .Register<MagicPanel, string?>(nameof(Layout));

        public string? LayoutStyle
        {
            get => GetValue(LayoutStyleProperty);
            set => SetValue(LayoutStyleProperty, value);
        }
        public static readonly StyledProperty<string?> LayoutStyleProperty = AvaloniaProperty
            .Register<MagicPanel, string?>(nameof(LayoutStyle));

        #endregion


        
        #region Ctor

        static MagicPanel()
        {
            AffectsMeasure<MagicPanel>(
                SpacingProperty,
                JustifyContentProperty,
                AlignItemsProperty,
                AlignContentProperty,
                OrientationProperty,
                ReverseProperty,
                FlexWrapProperty);
            
            AffectsParentMeasure<MagicPanel>(
                AlignSelfProperty,
                OrderProperty,
                FlexGrowProperty,
                FlexShrinkProperty,
                FlexBasisProperty);

            AffectsParentArrange<MagicPanel>(
                LayoutEx.ArrangedWidthProperty,
                LayoutEx.ArrangedHeightProperty,
                LayoutEx.ArrangedLeftProperty,
                LayoutEx.ArrangedTopProperty);
            
            RegisterLayout(StackLayout.Default);
            RegisterLayout(FlexLayout.Default);
            RegisterLayout(WrapLayout.Default);
            RegisterLayout(CanvasLayout.Default);

            LayoutProperty.Changed.AddClassHandler<MagicPanel>((panel, args) =>
            {
                panel.ResetLayout();
                panel.InvalidateMeasure();
            });

            LayoutStyleProperty.Changed.AddClassHandler<MagicPanel>((panel, args) =>
            {
                panel.ResetStyle();
                panel.InvalidateMeasure();
            });
        }

        public MagicPanel()
        {
            // TODO DELETE
            {
                this.Background = new SolidColorBrush(Colors.IndianRed, 0.1);
            }
        }

        private void ResetLayout()
        {
            if (Layout == null)
            {
                _layout = StackLayout.Default;
                return;
            }

            _layout = InternalLayouts.TryGetValue(Layout, out var layoutDefinition)
                ? layoutDefinition
                : StackLayout.Default;
        }

        private void ResetStyle()
        {
            if (LayoutStyle == null)
            {
                return;
            }

            var setterPairs = LayoutStyle.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var setterPair in setterPairs)
            {
                var setter = ParseSetter(setterPair);
                if (setter == null)
                {
                    continue;
                }

                _layout.ApplySetter(this, setter.Value.Item1.ToLower(), setter.Value.Item2);
            }
        }

        private static (string, string)? ParseSetter(string setterPair)
        {
            var pair = setterPair.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (pair.Length == 1)
            {
                return new ValueTuple<string, string>(setterPair, string.Empty);
            }

            return new ValueTuple<string, string>(pair[0].Trim(), pair[1].Trim());
        }

        #endregion


        
        #region Measure & Arrange

        protected override Size MeasureCore(Size availableSize)
        {
            if (IsVisible)
            {
                var margin            = Margin;
                var useLayoutRounding = UseLayoutRounding;
                var scale             = 1.0;

                if (useLayoutRounding)
                {
                    scale  = global::Avalonia.Layout.LayoutHelper.GetLayoutScale(this);
                    margin = global::Avalonia.Layout.LayoutHelper.RoundLayoutThickness(margin, scale, scale);
                }

                ApplyStyling();
                ApplyTemplate();

                var constrained = global::Avalonia.Layout.LayoutHelper.ApplyLayoutConstraints(
                    this,
                    availableSize.Deflate(margin));
                var measured = MeasureOverride(constrained);

                var width  = measured.Width;
                var height = measured.Height;

                {
                    var widthCache = Width;

                    if (!double.IsNaN(widthCache))
                    {
                        width = widthCache;
                    }
                }

                width = Math.Min(width, MaxWidth);
                width = Math.Max(width, MinWidth);

                {
                    var heightCache = Height;

                    if (!double.IsNaN(heightCache))
                    {
                        height = heightCache;
                    }
                }

                height = Math.Min(height, MaxHeight);
                height = Math.Max(height, MinHeight);

                if (useLayoutRounding)
                {
                    (width, height) = global::Avalonia.Layout.LayoutHelper.RoundLayoutSizeUp(new Size(width, height), scale, scale);
                }

                width  = Math.Min(width,  availableSize.Width);
                height = Math.Min(height, availableSize.Height);

                return NonNegative(new Size(width, height).Inflate(margin));
            }
            else
            {
                return new Size();
            }
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            if (IsVisible)
            {
                var useLayoutRounding = UseLayoutRounding;
                var scale             = global::Avalonia.Layout.LayoutHelper.GetLayoutScale(this);

                var margin  = Margin;
                var originX = finalRect.X + margin.Left;
                var originY = finalRect.Y + margin.Top;

                // Margin has to be treated separately because the layout rounding function is not linear
                // f(a + b) != f(a) + f(b)
                // If the margin isn't pre-rounded some sizes will be offset by 1 pixel in certain scales.
                if (useLayoutRounding)
                {
                    margin = global::Avalonia.Layout.LayoutHelper.RoundLayoutThickness(margin, scale, scale);
                }

                var availableSizeMinusMargins = new Size(
                    Math.Max(0, finalRect.Width - margin.Left - margin.Right),
                    Math.Max(0, finalRect.Height - margin.Top - margin.Bottom));
                var horizontalAlignment = HorizontalAlignment;
                var verticalAlignment   = VerticalAlignment;
                var size                = availableSizeMinusMargins;

                if (horizontalAlignment != HorizontalAlignment.Stretch)
                {
                    size = size.WithWidth(Math.Min(size.Width, DesiredSize.Width - margin.Left - margin.Right));
                }

                if (verticalAlignment != VerticalAlignment.Stretch)
                {
                    size = size.WithHeight(Math.Min(size.Height, DesiredSize.Height - margin.Top - margin.Bottom));
                }

                size = global::Avalonia.Layout.LayoutHelper.ApplyLayoutConstraints(this, size);

                if (useLayoutRounding)
                {
                    size = global::Avalonia.Layout.LayoutHelper.RoundLayoutSizeUp(size, scale, scale);
                    availableSizeMinusMargins = global::Avalonia.Layout.LayoutHelper.RoundLayoutSizeUp(availableSizeMinusMargins, scale, scale);
                }

                size = ArrangeOverride(size).Constrain(size);

                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                    case HorizontalAlignment.Stretch:
                        originX += (availableSizeMinusMargins.Width - size.Width) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        originX += availableSizeMinusMargins.Width - size.Width;
                        break;
                }

                switch (verticalAlignment)
                {
                    case VerticalAlignment.Center:
                    case VerticalAlignment.Stretch:
                        originY += (availableSizeMinusMargins.Height - size.Height) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        originY += availableSizeMinusMargins.Height - size.Height;
                        break;
                }

                if (useLayoutRounding)
                {
                    originX = global::Avalonia.Layout.LayoutHelper.RoundLayoutValue(originX, scale);
                    originY = global::Avalonia.Layout.LayoutHelper.RoundLayoutValue(originY, scale);
                }

                Bounds = new Rect(originX, originY, size.Width, size.Height);
            }
        }

        private static Size NonNegative(Size size)
        {
            return new Size(Math.Max(size.Width, 0), Math.Max(size.Height, 0));
        }

        #endregion


        
        #region MeasureOverride & ArrangeOverride

        private IReadOnlyList<Control> GetVisibleChildren()
        {
            var children = this.Children.Where(c => c.IsVisible);
            return GetReverse(this) 
                ? children.OrderBy(GetOrder).Reverse().ToList() 
                : children.OrderBy(GetOrder).ToList();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var children = GetVisibleChildren();
            return children.Count == 0 ? new Size() : _layout.MeasureOverride(this, children, availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = GetVisibleChildren();
            return children.Count == 0 ? new Size() : _layout.ArrangeOverride(this, children, finalSize);
        }

        #endregion


        
        #region INavigableContainer

        IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
        {
            return _layout.GetNavigatedControl(this, direction, from, wrap);
        }

        #endregion



        #region IScrollSnapPointsInfo

        // private bool _areHorizontalSnapPointsRegular;
        // private bool _areVerticalSnapPointsRegular;
        //
        // IReadOnlyList<double> IScrollSnapPointsInfo.GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment)
        // {
        //     
        // }
        //
        // double IScrollSnapPointsInfo.GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment, out double offset)
        // {
        // }
        //
        // bool IScrollSnapPointsInfo.AreHorizontalSnapPointsRegular
        // {
        //     get => _areHorizontalSnapPointsRegular;
        //     set => _areHorizontalSnapPointsRegular = value;
        // }
        //
        // bool IScrollSnapPointsInfo.AreVerticalSnapPointsRegular
        // {
        //     get => _areVerticalSnapPointsRegular;
        //     set => _areVerticalSnapPointsRegular = value;
        // }
        //
        // event EventHandler<RoutedEventArgs>? IScrollSnapPointsInfo.HorizontalSnapPointsChanged
        // {
        //     add => throw new NotImplementedException();
        //     remove => throw new NotImplementedException();
        // }
        //
        // event EventHandler<RoutedEventArgs>? IScrollSnapPointsInfo.VerticalSnapPointsChanged
        // {
        //     add => throw new NotImplementedException();
        //     remove => throw new NotImplementedException();
        // }

        #endregion
    }
}