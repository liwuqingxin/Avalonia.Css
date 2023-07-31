﻿using System;
using Avalonia;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css
{
    internal class TransitionsParser
    {
        public static Transitions? Parse(ICssBuilder builder, string transitionsString)
        {
            var interpreter    = builder.Interpreter;
            var transitions    = new Transitions();
            var transitionList = transitionsString.Trim('[', ']', ' ').Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var transition in transitionList)
            {
                if (interpreter.IsVar(transition, out var key) && Application.Current != null)
                {
                    //
                    // NOTE TryFindResource will make it static but dynamic.
                    //
                    if (builder.ResourceProvidersManager.TryFindResource<ITransition>(key!, Application.Current.ActualThemeVariant, out var resource))
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
