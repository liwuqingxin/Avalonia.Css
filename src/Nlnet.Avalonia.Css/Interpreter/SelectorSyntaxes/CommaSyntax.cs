using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    public class CommaSyntax : ISyntax
    {
        public Selector? ToSelector(Selector? previous)
        {
            throw new InvalidOperationException($"Do not call {nameof(ToSelector)}() of {nameof(CommaSyntax)}.");
        }
    }
}
