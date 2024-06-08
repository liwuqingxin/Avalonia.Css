using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Nlnet.Avalonia;

public interface IMaCa
{
    public double MaV(Size size);
  
    public double CaV(Size size);
    
    public double MaV(Point p);
    
    public double CaV(Point p);
    
    public double MaV(Control control);
    
    public double CaV(Control control);
    
    public void WithMav(ref Size size, double value);
    
    public void WithCav(ref Size size, double value);
    
    public void WithMav(ref double width, ref double height, double value);
    
    public void WithCav(ref double width, ref double height, double value);

    public void AccumulateMav(ref double width, ref double height, double value);
    
    public void MaxCav(ref double   width, ref double height, double value);
    
    double TileXOrAlign(double x, double alignPoint);
    
    double TileYOrAlign(double y, double alignPoint);
}

public static class MaCaExtension
{
    public static IMaCa GetMaCa(this Orientation orientation)
    {
        return orientation == Orientation.Horizontal ? HorizontalMaCa.Default : VerticalMaCa.Default;
    }
}

public class HorizontalMaCa : IMaCa
{
    public static HorizontalMaCa Default { get; } = new();
    
    private HorizontalMaCa()
    {
        
    }

    double IMaCa.MaV(Size size) => size.Width;

    double IMaCa.CaV(Size size) => size.Height;

    double IMaCa.MaV(Point p) => p.X;

    double IMaCa.CaV(Point p) => p.Y;

    double IMaCa.MaV(Control control) => control.Width;

    double IMaCa.CaV(Control control) => control.Height;

    void IMaCa.WithMav(ref Size size, double value) => size = size.WithWidth(value);

    void IMaCa.WithCav(ref Size size, double value) => size = size.WithHeight(value);

    void IMaCa.WithMav(ref double width, ref double height, double value) => width = value;
    
    void IMaCa.WithCav(ref double width, ref double height, double value) => height = value;

    void IMaCa.AccumulateMav(ref double width, ref double height, double value) => width += value;

    void IMaCa.MaxCav(ref double width, ref double height, double value) => height = Math.Max(height, value);
    
    double IMaCa.TileXOrAlign(double x, double alignPoint) => x;

    double IMaCa.TileYOrAlign(double y, double alignPoint) => alignPoint;
}

public class VerticalMaCa : IMaCa
{
    public static VerticalMaCa Default { get; } = new();
    
    private VerticalMaCa()
    {
        
    }
    
    double IMaCa.MaV(Size size) => size.Height;

    double IMaCa.CaV(Size size) => size.Width;

    double IMaCa.MaV(Point p) => p.Y;

    double IMaCa.CaV(Point p) => p.X;

    double IMaCa.MaV(Control control) => control.Height;

    double IMaCa.CaV(Control control) => control.Width;
    
    void IMaCa.WithMav(ref Size size, double value) => size = size.WithHeight(value);

    void IMaCa.WithCav(ref Size size, double value) => size = size.WithWidth(value);
    
    void IMaCa.WithMav(ref double width, ref double height, double value) => height = value;
    
    void IMaCa.WithCav(ref double width, ref double height, double value) => width = value;
    
    void IMaCa.AccumulateMav(ref double width, ref double height, double value) => height += value;

    void IMaCa.MaxCav(ref double width, ref double height, double value) => width = Math.Max(width, value);
    
    double IMaCa.TileXOrAlign(double x, double alignPoint) => alignPoint;

    double IMaCa.TileYOrAlign(double y, double alignPoint) => y;
}