using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Nlnet.Avalonia.Controls
{
    public sealed class MagicPanel : Canvas, INavigableContainer/*, IScrollSnapPointsInfo*/
    {
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



        private IMagicLayout _layout = StackLayout.Default;


        
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
            RegisterLayout(StackLayout.Default);
            RegisterLayout(new WrapLayout());
            RegisterLayout(new CanvasLayout());

            LayoutProperty.Changed.AddClassHandler<MagicPanel>((panel, args) =>
            {
                panel.InvalidateLayout();
                panel.InvalidateMeasure();
            });

            LayoutStyleProperty.Changed.AddClassHandler<MagicPanel>((panel, args) =>
            {
                panel.ParseStyle();
                panel.InvalidateMeasure();
            });
        }

        public MagicPanel()
        {
            this.Background = new SolidColorBrush(Colors.IndianRed, 0.1);
        }

        private void InvalidateLayout()
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

        #endregion



        #region Style
        
        private void ParseStyle()
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

                _layout.ApplySetter(this, setter.Value.Item1, setter.Value.Item2);
            }
        }

        private static (string,string)? ParseSetter(string setterPair)
        {
            var pair = setterPair.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (pair.Length != 2)
            {
                return null;
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
                    scale  = LayoutHelper.GetLayoutScale(this);
                    margin = LayoutHelper.RoundLayoutThickness(margin, scale, scale);
                }

                ApplyStyling();
                ApplyTemplate();

                var constrained = LayoutHelper.ApplyLayoutConstraints(
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
                    (width, height) = LayoutHelper.RoundLayoutSizeUp(new Size(width, height), scale, scale);
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
                var scale             = LayoutHelper.GetLayoutScale(this);

                var margin  = Margin;
                var originX = finalRect.X + margin.Left;
                var originY = finalRect.Y + margin.Top;

                // Margin has to be treated separately because the layout rounding function is not linear
                // f(a + b) != f(a) + f(b)
                // If the margin isn't pre-rounded some sizes will be offset by 1 pixel in certain scales.
                if (useLayoutRounding)
                {
                    margin = LayoutHelper.RoundLayoutThickness(margin, scale, scale);
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

                size = LayoutHelper.ApplyLayoutConstraints(this, size);

                if (useLayoutRounding)
                {
                    size                      = LayoutHelper.RoundLayoutSizeUp(size,                      scale, scale);
                    availableSizeMinusMargins = LayoutHelper.RoundLayoutSizeUp(availableSizeMinusMargins, scale, scale);
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
                    originX = LayoutHelper.RoundLayoutValue(originX, scale);
                    originY = LayoutHelper.RoundLayoutValue(originY, scale);
                }

                Bounds = new Rect(originX, originY, size.Width, size.Height);
            }
        }
        
        private static Size NonNegative(Size size)
        {
            return new Size(Math.Max(size.Width, 0), Math.Max(size.Height, 0));
        }

        #endregion


        
        #region Measure & Arrange Override

        protected override Size MeasureOverride(Size availableSize)
        {
            return _layout.MeasureOverride(this, availableSize, Children);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return _layout.ArrangeOverride(this, finalSize, this.Children);
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