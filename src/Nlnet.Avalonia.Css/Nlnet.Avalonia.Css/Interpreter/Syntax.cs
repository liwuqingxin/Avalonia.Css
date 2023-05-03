using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    public interface ISyntax
    {
        public Selector? ToSelector(Selector? previous);
    }

    public interface ITypeSyntax
    {
        string TypeName { get; set; }

        string Xmlns { get; set; }
    }

    public class OfTypeSyntax : ISyntax, ITypeSyntax
    {
        public string TypeName { get; set; } = string.Empty;

        public string Xmlns { get; set; } = string.Empty;

        public Selector? ToSelector(Selector? previous)
        {
            if (TypeResolver.Instance.TryGetType(TypeName, out var type))
            {
                return previous.OfType(type!);
            }

            this.WriteLine($"Can not resolve type '{TypeName}'");

            return previous;
        }
    }

    public class AttachedPropertySyntax : ISyntax, ITypeSyntax
    {
        public string Xmlns { get; set; } = string.Empty;

        public string TypeName { get; set; } = string.Empty;

        public string Property { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public Selector? ToSelector(Selector? previous)
        {
            if (TypeResolver.Instance.TryGetType(TypeName, out var type))
            {
                var avaloniaProperty = InterpreterHelper.GetAvaloniaProperty(type!, Property);
                if (avaloniaProperty == null)
                {
                    return previous;
                }
                var value = InterpreterHelper.ParseValue(avaloniaProperty, Value);
                if (value != null)
                {
                    return previous.PropertyEquals(avaloniaProperty, value);
                }
            }

            return previous;
        }
    }

    public class IsSyntax : ISyntax, ITypeSyntax
    {
        public string TypeName { get; set; } = string.Empty;

        public string Xmlns { get; set; } = string.Empty;

        public Selector? ToSelector(Selector? previous)
        {
            if (TypeResolver.Instance.TryGetType(TypeName, out var type))
            {
                return previous.Is(type!);
            }

            return previous;
        }
    }

    public class ClassSyntax : ISyntax
    {
        public string Class { get; set; } = string.Empty;

        public Selector? ToSelector(Selector? previous)
        {
            return previous.Class(Class);
        }
    }

    public class NameSyntax : ISyntax
    {
        public string Name { get; set; } = string.Empty;

        public Selector? ToSelector(Selector? previous)
        {
            return previous.Name(Name);
        }
    }

    public class PropertySyntax : ISyntax
    {
        public string Property { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public Selector? ToSelector(Selector? previous)
        {
            if (previous?.TargetType != null)
            {
                var avaloniaProperty = InterpreterHelper.GetAvaloniaProperty(previous.TargetType, Property);
                if (avaloniaProperty == null)
                {
                    return previous;
                }
                var value = InterpreterHelper.ParseValue(avaloniaProperty, Value);
                if (value != null)
                {
                    return previous.PropertyEquals(avaloniaProperty, value);
                }
            }

            this.WriteLine($"Previous selector's TargetType is null. '[{Property}={Value}]' skipped.");

            return previous;
        }
    }

    public class ChildSyntax : ISyntax
    {
        public Selector? ToSelector(Selector? previous)
        {
            return previous?.Child();
        }
    }

    public class DescendantSyntax : ISyntax
    {
        public Selector? ToSelector(Selector? previous)
        {
            return previous?.Descendant();
        }
    }

    public class TemplateSyntax : ISyntax
    {
        public Selector? ToSelector(Selector? previous)
        {
            return previous?.Template();
        }
    }

    public class NotSyntax : ISyntax
    {
        public IEnumerable<ISyntax> Argument { get; set; } = Enumerable.Empty<ISyntax>();

        public Selector? ToSelector(Selector? previous)
        {
            var selector = InterpreterHelper.ToSelector(Argument);
            if (selector == null)
            {
                this.WriteLine($"Can not apply :not selector for {Argument}");
                return previous;
            }

            return previous?.Not(selector);
        }
    }

    public class NthChildSyntax : ISyntax
    {
        public int Offset { get; set; }
        public int Step { get; set; }

        public Selector? ToSelector(Selector? previous)
        {
            return previous?.NthChild(Step, Offset);
        }
    }

    public class NthLastChildSyntax : ISyntax
    {
        public int Offset { get; set; }
        public int Step { get; set; }

        public Selector? ToSelector(Selector? previous)
        {
            return previous?.NthLastChild(Step, Offset);
        }
    }

    public class CommaSyntax : ISyntax
    {
        public Selector? ToSelector(Selector? previous)
        {
            throw new InvalidOperationException($"Do not call {nameof(ToSelector)}() of {nameof(CommaSyntax)}.");
        }
    }
}
