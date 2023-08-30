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

        var popupRoot = this.FindAncestorOfType<PopupRoot>();
        if (popupRoot?.Parent is not Popup popup)
        {
            return;
        }

        if (this.Child is not Visual visual)
        {
            return;
        }

        var point = e.GetPosition(visual);
        if (visual.Bounds.Contains(point))
        {
            return;
        }
        
        popup.SetCurrentValue(Popup.IsOpenProperty, false);
    }
}