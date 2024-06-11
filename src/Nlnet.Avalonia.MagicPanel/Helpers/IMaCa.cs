using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Nlnet.Avalonia;

public interface IMaCa
{
    public double MaV(double x, double y);
    
    public double CaV(double x, double y);
    
    public double MaV(Size size);
  
    public double CaV(Size size);
    
    public double MaV(Point p);
    
    public double CaV(Point p);
    
    public double MaV(Control control);
    
    public double CaV(Control control);
    
    public void WithMav(ref Size size, double value);
    
    public void WithCav(ref Size size, double value);
    
    public void WithMav(ref double x, ref double y, double value);
    
    public void WithCav(ref double x, ref double y, double value);

    public void AccumulateMav(ref double x, ref double y, double value);
    
    public void MaxCav(ref double x, ref double y, double value);

    void Align(Control control, double alignPoint);

    void Tile(Control control, double tilePoint);
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
    
    double IMaCa.MaV(double x, double y) => x;

    double IMaCa.CaV(double x, double y) => y;

    double IMaCa.MaV(Size size) => size.Width;

    double IMaCa.CaV(Size size) => size.Height;

    double IMaCa.MaV(Point p) => p.X;

    double IMaCa.CaV(Point p) => p.Y;

    double IMaCa.MaV(Control control) => control.Width;

    double IMaCa.CaV(Control control) => control.Height;

    void IMaCa.WithMav(ref Size size, double value) => size = size.WithWidth(value);

    void IMaCa.WithCav(ref Size size, double value) => size = size.WithHeight(value);

    void IMaCa.WithMav(ref double x, ref double y, double value) => x = value;
    
    void IMaCa.WithCav(ref double x, ref double y, double value) => y = value;

    void IMaCa.AccumulateMav(ref double x, ref double y, double value) => x += value;

    void IMaCa.MaxCav(ref double x, ref double y, double value) => y = Math.Max(y, value);

    void IMaCa.Align(Control control, double alignPoint) => LayoutEx.SetArrangedTop(control, alignPoint);
    
    void IMaCa.Tile(Control control, double tilePoint) => LayoutEx.SetArrangedLeft(control, tilePoint);
}

public class VerticalMaCa : IMaCa
{
    public static VerticalMaCa Default { get; } = new();
    
    private VerticalMaCa()
    {
        
    }
    
    double IMaCa.MaV(double x, double y) => y;

    double IMaCa.CaV(double x, double y) => x;
    
    double IMaCa.MaV(Size size) => size.Height;

    double IMaCa.CaV(Size size) => size.Width;

    double IMaCa.MaV(Point p) => p.Y;

    double IMaCa.CaV(Point p) => p.X;

    double IMaCa.MaV(Control control) => control.Height;

    double IMaCa.CaV(Control control) => control.Width;
    
    void IMaCa.WithMav(ref Size size, double value) => size = size.WithHeight(value);

    void IMaCa.WithCav(ref Size size, double value) => size = size.WithWidth(value);
    
    void IMaCa.WithMav(ref double x, ref double y, double value) => y = value;
    
    void IMaCa.WithCav(ref double x, ref double y, double value) => x = value;
    
    void IMaCa.AccumulateMav(ref double x, ref double y, double value) => y += value;

    void IMaCa.MaxCav(ref double x, ref double y, double value) => x = Math.Max(x, value);
    
    void IMaCa.Align(Control control, double alignPoint) => LayoutEx.SetArrangedLeft(control, alignPoint);
    
    void IMaCa.Tile(Control control, double tilePoint) => LayoutEx.SetArrangedTop(control, tilePoint);
}