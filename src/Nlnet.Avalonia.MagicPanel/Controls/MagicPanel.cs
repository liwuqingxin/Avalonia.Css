using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Nlnet.Avalonia.Controls
{
    public sealed class MagicPanel : Canvas
    {
        #region Definitions

        private static readonly Dictionary<string, ILayoutDefinition> InternalDefinitions = new(StringComparer.OrdinalIgnoreCase);

        public static IReadOnlyDictionary<string, ILayoutDefinition> Definitions => InternalDefinitions;

        public static void RegisterLayoutDefinition(ILayoutDefinition definition)
        {
            var name = definition.GetName();
            if (InternalDefinitions.TryGetValue(name, out _))
            {
                throw new InvalidOperationException($"Layout definition with same name of '{name}' has existed.");
            }
        }

        #endregion



        private ILayoutDefinition _layout = DefaultLayoutDefinition.Default;



        #region Properties

        public string Layout
        {
            get => GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }
        public static readonly StyledProperty<string> LayoutProperty = AvaloniaProperty
            .Register<MagicPanel, string>(nameof(Layout));

        #endregion


        
        #region Ctor

        static MagicPanel()
        {
            RegisterLayoutDefinition(DefaultLayoutDefinition.Default);

            LayoutProperty.Changed.AddClassHandler<MagicPanel>((panel, args) =>
            {
                panel._layout = panel.GetLayoutDefinition();
                panel.InvalidateMeasure();
            });
        }

        public MagicPanel()
        {
            this.Background = new SolidColorBrush(Colors.IndianRed, 0.1);
        }

        private ILayoutDefinition GetLayoutDefinition()
        {
            if (InternalDefinitions.TryGetValue(Layout, out var layoutDefinition))
            {
                return layoutDefinition;
            }

            return DefaultLayoutDefinition.Default;
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
                var scale = LayoutHelper.GetLayoutScale(this);

                var margin = Margin;
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
                var verticalAlignment = VerticalAlignment;
                var size = availableSizeMinusMargins;

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
                    size = LayoutHelper.RoundLayoutSizeUp(size, scale, scale);
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

        #endregion



        #region Measure & Arrange Override

        protected override Size MeasureOverride(Size availableSize)
        {
            return _layout.MeasureOverride(availableSize, Children);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _layout.ArrangeOverride(finalSize, this.Children);
            
            foreach (var child in this.Children)
            {
                this.ArrangeChild(child, finalSize);
            }

            return finalSize;
        }

        protected override void ArrangeChild(Control child, Size finalSize)
        {
            base.ArrangeChild(child, finalSize);
        }

        protected override void OnMeasureInvalidated()
        {
            base.OnMeasureInvalidated();
        }

        #endregion
        
        

        #region Utils

        private static Size NonNegative(Size size)
        {
            return new Size(Math.Max(size.Width, 0), Math.Max(size.Height, 0));
        }

        #endregion
    }
}