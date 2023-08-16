using Avalonia.Media;

namespace Nlnet.Avalonia.SampleAssistant;

public interface ICodeStyleContext
{
    public string Code { get; set; }
    public IBrush Foreground { get; set; }
    public Typeface Typeface { get; set; }
    public double FontSize { get; set; }
}
