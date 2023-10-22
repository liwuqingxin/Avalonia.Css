using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css.Controls
{
    public class TemplatedControlExtension : AvaloniaObject
    {
        public static void Use()
        {

        }

        static TemplatedControlExtension()
        {
            StyledElement.TemplatedParentProperty.Changed.AddClassHandler<StyledElement>(TemplatedParentChanged);
        }

        private static void TemplatedParentChanged(StyledElement styledElement, AvaloniaPropertyChangedEventArgs args)
        {
            if (styledElement.Classes is IPseudoClasses pseudo)
            {
                pseudo.Set(":is-part", styledElement.TemplatedParent != null);
            }
        }
    }
}
