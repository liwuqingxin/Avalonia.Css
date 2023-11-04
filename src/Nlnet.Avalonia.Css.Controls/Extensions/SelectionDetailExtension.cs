using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Nlnet.Avalonia.Css.Controls;

public static class SelectionDetailExtension
{
    public static void Use()
    {

    }

    private const string LeaveSmallerClass = ":sel-detail-leave-smaller";
    private const string LeaveLagerClass = ":sel-detail-leave-lager";
    private const string EnterSmallerClass = ":sel-detail-enter-smaller";
    private const string EnterLagerClass = ":sel-detail-enter-lager";

    static SelectionDetailExtension()
    {
        SelectingItemsControl.SelectedIndexProperty.Changed.AddClassHandler<SelectingItemsControl>(OnSelectedIndexChanged);
    }

    private static void OnSelectedIndexChanged(SelectingItemsControl itemsControl, AvaloniaPropertyChangedEventArgs arg)
    {
        var oldIndex = arg.GetOldValue<int>();
        var newIndex = arg.GetNewValue<int>();

        var oldPseudoClass = oldIndex < newIndex ? LeaveSmallerClass : LeaveLagerClass;
        var newPseudoClass = newIndex < oldIndex ? EnterSmallerClass : EnterLagerClass;

        if (itemsControl.ContainerFromIndex(oldIndex)?.Classes is IPseudoClasses oldClasses)
        {
            oldClasses.Remove(LeaveSmallerClass);
            oldClasses.Remove(LeaveLagerClass);
            oldClasses.Remove(EnterSmallerClass);
            oldClasses.Remove(EnterLagerClass);
            oldClasses.Add(oldPseudoClass);
        }

        if (itemsControl.ContainerFromIndex(newIndex)?.Classes is IPseudoClasses newClasses)
        {
            newClasses.Remove(LeaveSmallerClass);
            newClasses.Remove(LeaveLagerClass);
            newClasses.Remove(EnterSmallerClass);
            newClasses.Remove(EnterLagerClass);
            newClasses.Add(newPseudoClass);
        }
    }
}