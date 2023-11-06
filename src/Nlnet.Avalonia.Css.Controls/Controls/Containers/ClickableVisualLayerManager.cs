using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css.Controls;

public class ClickableVisualLayerManager : VisualLayerManager
{
    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.Transparent, this.Bounds);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        
        var popupRoot = this.TemplatedParent as PopupRoot;
        if (popupRoot?.Parent is not Popup popup)
        {
            e.Handled = true;
            return;
        }

        if (this.Child is not Visual visual)
        {
            e.Handled = true;
            return;
        }

        var point = e.GetPosition(this);
        if (this.Bounds.Contains(point))
        {
            e.Handled = true;
            return;
        }
        
        popup.SetCurrentValue(Popup.IsOpenProperty, false);
    }
}