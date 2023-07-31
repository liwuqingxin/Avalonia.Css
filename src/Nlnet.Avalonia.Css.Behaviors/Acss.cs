using System;
using Avalonia;

namespace Nlnet.Avalonia.Css.Behaviors
{
    public partial class Acss : AvaloniaObject, IBehaviorDeclarer
    {
        private static void OnInstanceChanged(AvaloniaPropertyChangedEventArgs<AcssBehavior> args)
        {
            if (args.OldValue is { HasValue: true, Value: not null })
            {
                args.OldValue.Value.Detach(args.Sender);
            }
            if (args.NewValue is { HasValue: true, Value: not null })
            {
                args.NewValue.Value.Attach(args.Sender);
            }
        }
    }
}
