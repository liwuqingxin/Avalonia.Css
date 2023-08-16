using System;
using Avalonia;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css
{
    internal class TransitionsParser
    {
        public static Transitions? Parse(IAcssBuilder builder, string valueString)
        {
            var interpreter    = builder.Interpreter;
            var transitions    = new Transitions();
            var transitionList = valueString[1..^1].Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var transition in transitionList)
            {
                if (interpreter.IsVar(transition, out var key))
                {
                    //
                    // NOTE TryFindResource will make it static but dynamic.
                    //
                    if (builder.ResourceProvidersManager.TryFindResource<ITransition>(key!, out var resource))
                    {
                        transitions.Add(resource!);
                    }
                }
                else
                {
                    var t = interpreter.ParseTransition(transition, out var shouldDefer, out var keyDuration, out var keyDelay, out var keyEasing);
                    if (t == null)
                    {
                        continue;
                    }
                    if (shouldDefer)
                    {
                        t.ApplyVarForDuration(builder, keyDuration);
                        t.ApplyVarForDelay(builder, keyDelay);
                        t.ApplyVarForEasing(builder, keyEasing);
                    }
                    transitions.Add(t);
                }
            }

            return transitions;
        }
    }
}
