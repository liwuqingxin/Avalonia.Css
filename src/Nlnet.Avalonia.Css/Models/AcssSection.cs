using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css
{
    internal interface IAcssSection
    {
        public IAcssBuilder AcssBuilder { get; }

        public string Selector { get; set; }

        public IAcssSection? Parent { get; set; }

        public IEnumerable<IAcssSection>? Children { get; set; }

        public void InitialSection(IAcssParser parser, ReadOnlySpan<char> content);
    }

    internal abstract class AcssSection : IAcssSection
    {
        public IAcssBuilder AcssBuilder { get; }

        public string Selector { get; set; }

        public IAcssSection? Parent { get; set; }

        public IEnumerable<IAcssSection>? Children { get; set; }

        public abstract void InitialSection(IAcssParser parser, ReadOnlySpan<char> content);

        protected AcssSection(IAcssBuilder acssBuilder, string selector)
        {
            AcssBuilder = acssBuilder;
            Selector    = selector.Trim();
        }
    }
}
