using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Nlnet.Avalonia.Css;

internal static class WindowsVisitor
{
    public static void VisitWindows(Func<Window, bool> visitor, Action<Control>? singleViewVisitor = null)
    {
        switch (Application.Current?.ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime lifetime1:
                _ = lifetime1.Windows.Any(visitor.Invoke);
                break;
            case ISingleViewApplicationLifetime lifetime2:
            {
                if (singleViewVisitor != null && lifetime2.MainView != null)
                {
                    singleViewVisitor.Invoke(lifetime2.MainView);
                }

                break;
            }
        }
    }

    public static void VisitLifecycle(
        Action<IClassicDesktopStyleApplicationLifetime> classicLifeVisitor,
        Action<ISingleViewApplicationLifetime> singleLifeVisitor)
    {
        switch (Application.Current?.ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime lifetime1:
                classicLifeVisitor(lifetime1);
                break;
            case ISingleViewApplicationLifetime lifetime2:
                singleLifeVisitor(lifetime2);
                break;
        }
    }

    public static Window? GetActiveOrMainWindow()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        }

        return null;
    }
}