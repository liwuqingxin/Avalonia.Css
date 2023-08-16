using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Nlnet.Avalonia.Senior.Controls;

public class NtResizeThumb : Thumb
{
    public StandardCursorType CursorType
    {
        get { return GetValue(CursorTypeProperty); }
        set { SetValue(CursorTypeProperty, value); }
    }
    public static readonly StyledProperty<StandardCursorType> CursorTypeProperty = AvaloniaProperty
        .Register<NtResizeThumb, StandardCursorType>(nameof(CursorType));

    static NtResizeThumb()
    {
        CursorTypeProperty.Changed.AddClassHandler<NtResizeThumb>((thumb, args) =>
        {
            thumb.Cursor = Cursor.Parse(thumb.CursorType.ToString());
        });
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (this.TemplatedParent is not Window window)
        {
            return;
        }

        var windowEdge = CursorType switch
        {
            StandardCursorType.TopLeftCorner     => WindowEdge.NorthWest,
            StandardCursorType.TopSide           => WindowEdge.North,
            StandardCursorType.TopRightCorner    => WindowEdge.NorthEast,
            StandardCursorType.LeftSide          => WindowEdge.West,
            StandardCursorType.RightSide         => WindowEdge.East,
            StandardCursorType.BottomLeftCorner  => WindowEdge.SouthWest,
            StandardCursorType.BottomSide        => WindowEdge.South,
            StandardCursorType.BottomRightCorner => WindowEdge.SouthEast,
            StandardCursorType.Arrow             => throw new ArgumentOutOfRangeException(),
            StandardCursorType.Ibeam             => throw new ArgumentOutOfRangeException(),
            StandardCursorType.Wait              => throw new ArgumentOutOfRangeException(),
            StandardCursorType.Cross             => throw new ArgumentOutOfRangeException(),
            StandardCursorType.UpArrow           => throw new ArgumentOutOfRangeException(),
            StandardCursorType.SizeWestEast      => throw new ArgumentOutOfRangeException(),
            StandardCursorType.SizeNorthSouth    => throw new ArgumentOutOfRangeException(),
            StandardCursorType.SizeAll           => throw new ArgumentOutOfRangeException(),
            StandardCursorType.No                => throw new ArgumentOutOfRangeException(),
            StandardCursorType.Hand              => throw new ArgumentOutOfRangeException(),
            StandardCursorType.AppStarting       => throw new ArgumentOutOfRangeException(),
            StandardCursorType.Help              => throw new ArgumentOutOfRangeException(),
            StandardCursorType.DragMove          => throw new ArgumentOutOfRangeException(),
            StandardCursorType.DragCopy          => throw new ArgumentOutOfRangeException(),
            StandardCursorType.DragLink          => throw new ArgumentOutOfRangeException(),
            StandardCursorType.None              => throw new ArgumentOutOfRangeException(),
            _                                    => throw new ArgumentOutOfRangeException()
        };

        window.BeginResizeDrag(windowEdge, e);
    }
}
