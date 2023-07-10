using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css
{
    internal class TransitionsParser
    {
        public static Transitions? Parse(string transitionsString)
        {
            var resourceProviders = ServiceLocator.GetService<IResourceProvidersManager>();
            var interpreter       = ServiceLocator.GetService<ICssInterpreter>();
            var transitions       = new Transitions();
            var transitionList    = transitionsString.Trim('[', ']', ' ').Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var transition in transitionList)
            {
                if (interpreter.IsVar(transition, out var key) && Application.Current != null)
                {
                    //
                    // NOTE TryFindResource will make it static but dynamic.
                    //
                    if (resourceProviders.TryFindResource<ITransition>(key!, out var resource))
                    {
                        transitions.Add(resource!);
                    }
                }
                else
                {
                    var t = interpreter.ParseTransition(transition);
                    if (t != null)
                    {
                        transitions.Add(t);
                    }
                }
            }

            return transitions;
        }
    }
}
