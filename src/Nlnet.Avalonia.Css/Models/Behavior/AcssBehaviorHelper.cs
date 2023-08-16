using Avalonia;

namespace Nlnet.Avalonia.Css;

public static class AcssBehaviorHelper
{
    public static void OnInstanceChanged(AvaloniaPropertyChangedEventArgs<AcssBehavior> args)
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