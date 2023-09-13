using Avalonia.Controls;
using Avalonia.Threading;

namespace Nlnet.Avalonia.Senior.Controls;

public static class WindowExtensions
{
    public static void ShowDialogSync(this Window window, Window owner)
    {
        var frame = new DispatcherFrame();
        window.ShowDialog(owner).ContinueWith(t =>
        {
            frame.Continue = false;
        });
        Dispatcher.UIThread.PushFrame(frame);
    }
    
    public static T? ShowDialogSync<T>(this Window window, Window owner)
    {
        var frame = new DispatcherFrame();
        T? result = default;
        window.ShowDialog<T>(owner).ContinueWith(t =>
        {
            result = t.Result;
            frame.Continue = false;
        });
        Dispatcher.UIThread.PushFrame(frame);

        return result;
    }
}