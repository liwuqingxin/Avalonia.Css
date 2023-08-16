using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal class BehaviorTemplate : ITemplate
    {
        private readonly Func<AcssBehavior?>? _content;

        public BehaviorTemplate(Func<AcssBehavior?>? getter)
        {
            _content = getter;
        }

        public object? Build()
        {
            return _content?.Invoke();
        }
    }
}
