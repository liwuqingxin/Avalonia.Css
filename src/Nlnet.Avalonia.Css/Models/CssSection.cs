using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal interface ICssSection
    {
        public ICssBuilder CssBuilder { get; }

        public string Selector { get; set; }

        public ICssSection? Parent { get; set; }

        public IEnumerable<ICssSection>? Children { get; set; }

        public void InitialSection(ICssParser parser, ReadOnlySpan<char> content);
    }

    internal abstract class CssSection : ICssSection
    {
        public ICssBuilder CssBuilder { get; }

        public string Selector { get; set; }

        public ICssSection? Parent { get; set; }

        public IEnumerable<ICssSection>? Children { get; set; }

        public abstract void InitialSection(ICssParser parser, ReadOnlySpan<char> content);

        protected CssSection(ICssBuilder cssBuilder, string selector)
        {
            CssBuilder = cssBuilder;
            Selector   = selector.Trim();
        }
    }
}
