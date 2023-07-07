using System;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.SampleAssistant
{
    public class CodeEditor : TextBox, IStyleable
    {
        Type IStyleable.StyleKey => typeof(CodeEditor);
    }
}
