using System;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css.Controls
{
    public class Container : Border
    {
        protected override Type StyleKeyOverride { get; } = typeof(Container);

        public bool UsePointerReaction
        {
            get { return GetValue(UsePointerReactionProperty); }
            set { SetValue(UsePointerReactionProperty, value); }
        }
        public static readonly StyledProperty<bool> UsePointerReactionProperty = AvaloniaProperty
            .Register<Container, bool>(nameof(UsePointerReaction), false);
    }
}
