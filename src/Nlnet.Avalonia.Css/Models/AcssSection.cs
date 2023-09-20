using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css
{
    internal interface IAcssSection
    {
        public IAcssBuilder AcssBuilder { get; }

        public string Header { get; set; }

        public IAcssSection? Parent { get; set; }

        public IEnumerable<IAcssSection>? Children { get; set; }

        public void InitialSection(IAcssParser parser, ReadOnlySpan<char> content);
        
        IAcssSection Clone();
    }

    internal abstract class AcssSection : IAcssSection
    {
        public IAcssBuilder AcssBuilder { get; }

        public string Header { get; set; }

        public IAcssSection? Parent { get; set; }

        public IEnumerable<IAcssSection>? Children { get; set; }

        public abstract void InitialSection(IAcssParser parser, ReadOnlySpan<char> content);

        public abstract IAcssSection Clone();

        protected AcssSection(IAcssBuilder acssBuilder, string header)
        {
            AcssBuilder = acssBuilder;
            Header = header.Trim();
        }
    }
}
