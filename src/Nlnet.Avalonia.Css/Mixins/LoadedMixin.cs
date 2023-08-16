using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Nlnet.Avalonia.Css
{
    public static class LoadedMixin
    {
        // ReSharper disable once InconsistentNaming
        private const string Pseudo_IsLoaded = ":is-loaded";

        /// <summary>
        /// Sets the value that indicates if enable the loaded-state for the control.
        /// </summary>
        /// <param name="control">The animation setter.</param>
        /// <param name="use">The property animator value.</param>
        public static void SetAttach(Control control, bool use)
        {
            if (use == false)
            {
                control.Loaded -= ControlOnLoaded;
                control.Unloaded -= ControlOnUnloaded;
                return;
            }

            control.Loaded -= ControlOnLoaded;
            control.Unloaded -= ControlOnUnloaded;
            control.Loaded += ControlOnLoaded;
            control.Unloaded += ControlOnUnloaded;
        }

        private static void ControlOnLoaded(object? sender, RoutedEventArgs e)
        {
            if (sender is not Control control)
            {
                return;
            }

            if (control.Classes is IPseudoClasses pseudoClasses)
            {
                pseudoClasses.Set(Pseudo_IsLoaded, true);
            }
        }

        private static void ControlOnUnloaded(object? sender, RoutedEventArgs e)
        {
            if (sender is not Control control)
            {
                return;
            }

            if (control.Classes is IPseudoClasses pseudoClasses)
            {
                pseudoClasses.Set(Pseudo_IsLoaded, false);
            }
        }
    }
}