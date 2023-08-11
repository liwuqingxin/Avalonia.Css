using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Nlnet.Avalonia.Css;

internal static class PlatformService
{
    public static bool IsLinux   => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsOsx     => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    /// <summary>
    /// Just for scaling in linux.
    /// </summary>
    /// <param name="window"></param>
    public static void FixScalingInLinux(Window window)
    {
        if (window.IsLoaded)
        {
            FixWindowScaling(window);
        }
        else
        {
            window.Loaded += (sender, args) => FixWindowScaling(window);
        }
    }

    private static void FixWindowScaling(Window window)
    {
        if (IsLinux)
        {
            Dispatcher.UIThread.Post(() =>
            {
                window.Position = new PixelPoint(window.Position.X + 1, window.Position.Y);
                window.Position = new PixelPoint(window.Position.X - 1, window.Position.Y);
            });
        }
    }
}