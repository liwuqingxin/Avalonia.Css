using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Controls
{
    public class Container : Border, IStyleable
    {
        Type IStyleable.StyleKey => typeof(Container);

        public bool UsePointerReaction
        {
            get { return GetValue(UsePointerReactionProperty); }
            set { SetValue(UsePointerReactionProperty, value); }
        }
        public static readonly StyledProperty<bool> UsePointerReactionProperty = AvaloniaProperty
            .Register<Container, bool>(nameof(UsePointerReaction), false);
    }
}
