using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css
{
    internal interface IAcssSection
    {
        public IAcssSection? Parent { get; set; }

        public IEnumerable<IAcssSection>? Children { get; set; }

        public void InitialSection(IAcssParser parser, ReadOnlySpan<char> content);
        
        IAcssSection Clone();
    }

    internal abstract class AcssSection : IAcssSection
    {
        protected readonly IAcssContext Context;

        public string Header { get; set; }

        public IAcssSection? Parent { get; set; }

        public IEnumerable<IAcssSection>? Children { get; set; }

        public abstract void InitialSection(IAcssParser parser, ReadOnlySpan<char> content);

        public abstract IAcssSection Clone();

        protected AcssSection(IAcssContext context, string header)
        {
            Context = context;
            Header = header.Trim();
        }
    }
}
