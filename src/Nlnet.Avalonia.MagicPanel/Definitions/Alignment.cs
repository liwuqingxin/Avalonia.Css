using System;
using Avalonia.Layout;

namespace Nlnet.Avalonia;

public enum Alignment
{
    Stretch,
    Center,
    Start,
    End,
}

public static class AlignmentExtensions
{
    public static VerticalAlignment ToVertical(this Alignment alignment)
    {
        return alignment switch
        {
            Alignment.Stretch => VerticalAlignment.Stretch,
            Alignment.Center  => VerticalAlignment.Center,
            Alignment.Start   => VerticalAlignment.Top,
            Alignment.End     => VerticalAlignment.Bottom,
            _                 => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }
    
    public static HorizontalAlignment ToHorizontal(this Alignment alignment)
    {
        return alignment switch
        {
            Alignment.Stretch => HorizontalAlignment.Stretch,
            Alignment.Center  => HorizontalAlignment.Center,
            Alignment.Start   => HorizontalAlignment.Left,
            Alignment.End     => HorizontalAlignment.Right,
            _                 => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }
}