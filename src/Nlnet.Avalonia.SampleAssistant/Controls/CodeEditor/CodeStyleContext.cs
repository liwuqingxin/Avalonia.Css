using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.SampleAssistant;

public class CodeStyleContext<T> : ICodeStyleContext where T : Enum
{
    public string Code { get; set; }
    public IBrush Foreground { get; set; }
    public Typeface Typeface { get; set; }
    public double FontSize { get; set; }
    public T Semantics { get; set; }

    public CodeStyleContext(string code, IBrush foreground, Typeface typeface, double fontSize, T semantics)
    {
        Code = code;
        Foreground = foreground;
        Typeface = typeface;
        FontSize = fontSize;
        Semantics = semantics;
    }
}
