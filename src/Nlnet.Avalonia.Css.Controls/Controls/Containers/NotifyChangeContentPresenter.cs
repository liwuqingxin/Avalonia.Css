﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.Css.Controls;

[PseudoClasses(Pseudo_Changing, Pseudo_Changed)]
public class NotifyChangeContentPresenter : ContentPresenter
{
    private const string Pseudo_Changing = ":changing";
    private const string Pseudo_Changed = ":changed";

    static NotifyChangeContentPresenter()
    {
        ContentProperty.Changed.AddClassHandler<NotifyChangeContentPresenter>((container, args) =>
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