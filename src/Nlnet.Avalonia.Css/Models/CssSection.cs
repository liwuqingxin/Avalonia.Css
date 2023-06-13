﻿using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    public interface ICssSection
    {
        public string Selector { get; set; }

        public ICssSection? Parent { get; set; }

        public IEnumerable<ICssSection>? Children { get; set; }

        public void InitialSection(ICssParser parser, ReadOnlySpan<char> content);
    }

    public abstract class CssSection : ICssSection
    {
        public string Selector { get; set; }

        public ICssSection? Parent { get; set; }

        public IEnumerable<ICssSection>? Children { get; set; }

        public abstract void InitialSection(ICssParser parser, ReadOnlySpan<char> content);

        protected CssSection(string selector)
        {
            Selector = selector.Trim();
        }
    }
}
