using System;
using Avalonia.Controls;

namespace Nlnet.Avalonia.SampleAssistant
{
    public class CodeEditor : TextBox
    {
        protected override Type StyleKeyOverride { get; } = typeof(CodeEditor);
    }
}
