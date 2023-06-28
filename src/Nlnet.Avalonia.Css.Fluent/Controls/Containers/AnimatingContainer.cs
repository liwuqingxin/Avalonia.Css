using Avalonia.Styling;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;

namespace Nlnet.Avalonia.Css.Fluent
{
    [PseudoClasses(Pseudo_Changing, Pseudo_Changed)]
    public class AnimatingContainer : ContentControl, IStyleable
    {
        Type IStyleable.StyleKey => typeof(AnimatingContainer);

        private const string Pseudo_Changing = ":changing";
        private const string Pseudo_Changed = ":changed";

        public object? LeavingContent
        {
            get { return GetValue(LeavingContentProperty); }
            set { SetValue(LeavingContentProperty, value); }
        }
        public static readonly StyledProperty<object?> LeavingContentProperty = AvaloniaProperty
            .Register<AnimatingContainer, object?>(nameof(LeavingContent));

        static AnimatingContainer()
        {
            ContentProperty.Changed.AddClassHandler<AnimatingContainer>((container, args) =>
            {
                container.LeavingContent = args.OldValue;

                container.PseudoClasses.Set(Pseudo_Changing, false);
                container.PseudoClasses.Set(Pseudo_Changing, true);

                if (args.OldValue is Control oldControl)
                {
                    oldControl.Loaded -= container.OnContentLoaded;
                }
                if (container.Content is Control control)
                {
                    control.Loaded += container.OnContentLoaded;
                }
            });
        }

        private void OnContentLoaded(object? sender, RoutedEventArgs e)
        {
            PseudoClasses.Set(Pseudo_Changed, false);
            PseudoClasses.Set(Pseudo_Changed, true);
        }
    }
}
