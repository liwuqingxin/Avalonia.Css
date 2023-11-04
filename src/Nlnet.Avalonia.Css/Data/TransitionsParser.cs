using System;
using System.Linq;
using System.Reflection;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Nlnet.Avalonia.Css
{
    internal class TransitionsParser
    {
        // TODO Request public the ITransition.Property.
        private static readonly PropertyInfo? PropertyProp = typeof(ITransition).GetProperty("Property", BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// For setter value parser.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static Transitions? Parse(IAcssContext context, string valueString)
        {
            var config = context.GetService<IAcssConfiguration>();
            if (config.EnableTransitions == false)
            {
                return null;
            }

            var interpreter    = context.GetService<IAcssInterpreter>();
            var transitions    = new Transitions();
            var transitionList = valueString[1..^1].Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            var app            = Checks.CheckApplication();
            foreach (var transition in transitionList)
            {
                if (interpreter.IsVar(transition, out var key))
                {
                    app.GetResourceObservable(key!).Subscribe(new AnonymousObserver<object?>(o =>
                    {
                        if (o is not ITransition t || PropertyProp == null)
                        {
                            return;
                        }
                        var exist = transitions.FirstOrDefault(t1 => PropertyProp.GetValue(t1) == PropertyProp.GetValue(t));
                        if (exist != null)
                        {
                            transitions.Remove(exist);
                        }
                        transitions.Add(t);
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
