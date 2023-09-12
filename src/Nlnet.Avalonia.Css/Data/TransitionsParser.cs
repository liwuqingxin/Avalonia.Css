using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Nlnet.Avalonia.Css
{
    internal class TransitionsParser
    {
        /// <summary>
        /// For setter value parser.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static Transitions? Parse(IAcssBuilder builder, string valueString)
        {
            var interpreter    = builder.Interpreter;
            var transitions    = new Transitions();
            var transitionList = valueString[1..^1].Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            var app            = Checks.CheckApplication();
            foreach (var transition in transitionList)
            {
                if (interpreter.IsVar(transition, out var key))
                {
                    app.GetResourceObservable(key!).Subscribe(new AnonymousObserver<object?>(o =>
                    {
                        if (o is ITransition t)
                        {
                            transitions.Add(t);
                        }
                    }));
                }
                else
                {
                    var t = interpreter.ParseTransition(transition, app);
                    if (t == null)
                    {
                        continue;
                    }
                    transitions.Add(t);
                }
            }

            return transitions;
        }
    }
}
