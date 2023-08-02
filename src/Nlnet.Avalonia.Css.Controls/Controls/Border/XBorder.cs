using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Utilities;

namespace Nlnet.Avalonia.Css.Controls
{
    public class XBorder : Decorator
    {
        /// <summary>
        /// Defines the <see cref="Background"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush?> BackgroundProperty =
            AvaloniaProperty.Register<XBorder, IBrush?>(nameof(Background));

        /// <summary>
        /// Defines the <see cref="BorderBrush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush?> BorderBrushProperty =
            AvaloniaProperty.Register<XBorder, IBrush?>(nameof(BorderBrush));

        /// <summary>
        /// Defines the <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly StyledProperty<Thickness> BorderThicknessProperty =
            AvaloniaProperty.Register<XBorder, Thickness>(nameof(BorderThickness));

        /// <summary>
        /// Defines the <see cref="CornerRadius"/> property.
        /// </summary>
        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.Register<XBorder, CornerRadius>(nameof(CornerRadius));

        /// <summary>
        /// Defines the <see cref="BoxShadow"/> property.
        /// </summary>
        public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
            AvaloniaProperty.Register<XBorder, BoxShadows>(nameof(BoxShadow));

        /// <summary>
        /// Indicates if show the x border.
        /// </summary>
        public static readonly StyledProperty<bool> ShowXBorderProperty =
            AvaloniaProperty.Register<XBorder, bool>(nameof(ShowXBorder), true);

        /// <summary>
        /// Indicates the height of bottom edge.
        /// </summary>
        public static readonly StyledProperty<double> BottomEdgeHeightProperty = AvaloniaProperty
            .Register<XBorder, double>(nameof(BottomEdgeHeight));

        /// <summary>
        /// Defines the bottom edge brush.
        /// </summary>
        public static readonly StyledProperty<IBrush?> BottomEdgeBrushProperty = AvaloniaProperty
            .Register<XBorder, IBrush?>(nameof(BottomEdgeBrush));

        /// <summary>
        /// Indicates if show the bottom edge.
        /// </summary>
        public static readonly StyledProperty<bool> ShowBottomEdgeProperty = AvaloniaProperty
            .Register<XBorder, bool>(nameof(ShowBottomEdge), true);



        private readonly BorderRenderHelper _borderRenderHelper = new();
        private Thickness? _layoutThickness;
        private double _scale;

        /// <summary>
        /// Initializes static members of the <see cref="XBorder"/> class.
        /// </summary>
        static XBorder()
        {
            AffectsRender<XBorder>(
                BackgroundProperty,
                BorderBrushProperty,
                BorderThicknessProperty,
                CornerRadiusProperty,
                BoxShadowProperty,
                ShowXBorderProperty,
                BottomEdgeHeightProperty,
                BottomEdgeBrushProperty);
            AffectsMeasure<XBorder>(BorderThicknessProperty);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            switch (change.Property.Name)
            {
                case nameof(UseLayoutRounding):
                case nameof(BorderThickness):
                    _layoutThickness = null;
                    break;
                case nameof(CornerRadius):
                    
                    break;
            }
        }

        /// <summary>
        /// Gets or sets a brush with which to paint the background.
        /// </summary>
        public IBrush? Background
        {
            get => GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets a brush with which to paint the border.
        /// </summary>
        public IBrush? BorderBrush
        {
            get => GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get => GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the radius of the border rounded corners.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets the box shadow effect parameters
        /// </summary>
        public BoxShadows BoxShadow
        {
            get => GetValue(BoxShadowProperty);
            set => SetValue(BoxShadowProperty, value);
        }

        /// <summary>
        /// Gets or sets the value that indicates if show the x border.
        /// </summary>
        public bool ShowXBorder
        {
            get => GetValue(ShowXBorderProperty);
            set => SetValue(ShowXBorderProperty, value);
        }

        /// <summary>
        /// Gets or sets the height of bottom edge.
        /// </summary>
        public double BottomEdgeHeight
        {
            get { return GetValue(BottomEdgeHeightProperty); }
            set { SetValue(BottomEdgeHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the bottom edge brush.
        /// </summary>
        public IBrush? BottomEdgeBrush
        {
            get { return GetValue(BottomEdgeBrushProperty); }
            set { SetValue(BottomEdgeBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value that indicates if show the bottom edge.
        /// </summary>
        public bool ShowBottomEdge
        {
            get { return GetValue(ShowBottomEdgeProperty); }
            set { SetValue(ShowBottomEdgeProperty, value); }
        }

        private Thickness LayoutThickness
        {
            get
            {
                VerifyScale();

                if (_layoutThickness == null)
                {
                    var borderThickness = BorderThickness;

                    if (UseLayoutRounding)
                        borderThickness = LayoutHelper.RoundLayoutThickness(borderThickness, _scale, _scale);

                    _layoutThickness = borderThickness;
                }

                return _layoutThickness.Value;
            }
        }

        private void VerifyScale()
        {
            var currentScale = LayoutHelper.GetLayoutScale(this);
            if (MathUtilities.AreClose(currentScale, _scale))
                return;

            _scale = currentScale;
            _layoutThickness = null;
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        public sealed override void Render(DrawingContext context)
        {
            if (ShowXBorder)
            {
                var rect = new Rect(Bounds.X, Bounds.Bottom - 2, Bounds.Width, 2);
                var corner = CornerRadius.BottomLeft + 2;
                using (context.PushClip(rect))
                {
                    context.DrawRectangle(null, new Pen(Brush.Parse("#c1c1c1")), Bounds, corner,corner);
                }
            }

            _borderRenderHelper.Render(
                context,
                Bounds.Size,
                LayoutThickness,
                CornerRadius,
                Background,
                BorderBrush,
                BoxShadow);

            if (BottomEdgeHeight > 0 && ShowBottomEdge)
            {
                var rect = new Rect(Bounds.X-1, Bounds.Bottom - BottomEdgeHeight, Bounds.Width + 2, BottomEdgeHeight);
                var corner = CornerRadius.BottomLeft + 2;
                using (context.PushClip(rect))
                {
                    context.DrawRectangle(BottomEdgeBrush, null, Bounds, corner, corner);
                }
            }
        }

        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The desired size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return LayoutHelper.MeasureChild(Child, availableSize, Padding, BorderThickness);
        }

        /// <summary>
        /// Arranges the control's child.
        /// </summary>
        /// <param name="finalSize">The size allocated to the control.</param>
        /// <returns>The space taken.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return LayoutHelper.ArrangeChild(Child, finalSize, Padding, BorderThickness);
        }

        public CornerRadius ClipToBoundsRadius => CornerRadius;
    }
}
