using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Nlnet.Avalonia.Css.Controls;

public class DataGridScrollBar : ScrollBar
{
    protected override Type StyleKeyOverride { get; } = typeof(ScrollBar);
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var field = typeof(ScrollBar).GetField("_owner", BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(this, null);
    }
}