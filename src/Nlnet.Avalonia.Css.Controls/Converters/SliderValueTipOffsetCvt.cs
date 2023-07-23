using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css.Controls
{
    public class SliderValueTipOffsetCvt : IMultiValueConverter
    {
        public static SliderValueTipOffsetCvt HorizontalCvt { get; } = new()
        {
            _orientation = Orientation.Horizontal,
        };

        public static SliderValueTipOffsetCvt VerticalCvt { get; } = new()
        {
            _orientation = Orientation.Vertical,
        };

        private Orientation _orientation = Orientation.Horizontal;

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count >= 2  && values[0] is Rect bounds && values[1] is Popup popup)
            {
                var popupRoot = popup.Child?.FindAncestorOfType<PopupRoot>() ?? popup.Child;
                if (popupRoot == null)
                {
                    return BindingOperations.DoNothing;
                }

                return _orientation switch
                {
                    Orientation.Horizontal => bounds.X - popupRoot.Bounds.Width  / 2 + bounds.Width  / 2,
                    Orientation.Vertical   => bounds.Y - popupRoot.Bounds.Height / 2 + bounds.Height / 2,
                    _                      => BindingOperations.DoNothing,
                };
            }

            return BindingOperations.DoNothing;
        }
    }
}
