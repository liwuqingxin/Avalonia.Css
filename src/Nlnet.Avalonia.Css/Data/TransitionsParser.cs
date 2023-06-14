using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css
{
    public class TransitionsParser
    {
        public static Transitions? Parse(string transitionsString)
        {
            var transitions = new Transitions();
            var transitionList = transitionsString.Trim('[',']',' ').Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var transition in transitionList)
            {
                if (ServiceLocator.GetService<ICssInterpreter>().IsVar(transition, out var key) && Application.Current != null)
                {
                    if (Application.Current.TryFindResource(key!, out var resource) && resource is ITransition t)
                    {
                        transitions.Add(t);
                    }
                }
                else
                {
                    var t = ServiceLocator.GetService<ICssInterpreter>().ParseTransition(transition);
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
