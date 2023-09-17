using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Threading;
// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.Css.Controls;

[PseudoClasses(Pseudo_Changing, Pseudo_Changed)]
public class NotifyChangeContentControl : ContentControl
{
    protected override Type StyleKeyOverride { get; } = typeof(ContentControl);

    private const string Pseudo_Changing = ":changing";
    private const string Pseudo_Changed = ":changed";
    
    static NotifyChangeContentControl()
    {
        ContentProperty.Changed.AddClassHandler<NotifyChangeContentControl>((container, args) =>
        {
            container.PseudoClasses.Set(Pseudo_Changing, false);
            container.PseudoClasses.Set(Pseudo_Changing, true);

            container.SetCurrentValue(Control.OpacityProperty, 0);

            Dispatcher.UIThread.Post(() =>
            {
                container.PseudoClasses.Set(Pseudo_Changed, false);
                container.PseudoClasses.Set(Pseudo_Changed, true);
            }, DispatcherPriority.SystemIdle);
        });
    }
}