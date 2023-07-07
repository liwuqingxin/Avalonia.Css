using System.Collections.Generic;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Nlnet.Avalonia.SampleAssistant
{
    public interface ICodeHighlighter
    {
        public bool TryHighlight(string code, ICodeStyleContext context, out IReadOnlyList<ValueSpan<TextRunProperties>>? styles);
    }
}
