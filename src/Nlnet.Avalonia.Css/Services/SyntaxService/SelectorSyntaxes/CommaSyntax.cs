using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal class CommaSyntax : ISyntax
    {
        public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
        {
            throw new InvalidOperationException($"Do not call {nameof(ToSelector)}() of {nameof(CommaSyntax)}.");
        }
    }
}
