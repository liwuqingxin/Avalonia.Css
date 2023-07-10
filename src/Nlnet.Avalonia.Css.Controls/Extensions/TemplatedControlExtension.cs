using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css.Controls
{
    // TODO Try to remove this.
    public class TemplatedControlExtension : AvaloniaObject
    {
        static TemplatedControlExtension()
        {
            StyledElement.TemplatedParentProperty.Changed.AddClassHandler<StyledElement>(TemplatedParentChanged);
        }

        public static void Init()
        {

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
