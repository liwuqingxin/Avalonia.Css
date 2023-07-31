using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Nlnet.Avalonia.Css.CompileGenerator
{
    [Generator]
    public class BehaviorAttachedPropertyGenerator : ISourceGenerator
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
            
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (!(syntaxNode is ClassDeclarationSyntax classDeclarationSyntax) ||
                    classDeclarationSyntax.AttributeLists.Count <= 0)
                {
                    return;
                }

                var isBehavior = false;
                foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
                {
                    if (attributeListSyntax.Attributes.Select(attribute => attribute.Name.ToFullString()).Any(name => name == "Behavior"))
                    {
                        isBehavior = true;
                    }

                    if (isBehavior)
                    {
                        break;
                    }
                }

                if (isBehavior)
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();

            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            if (!(context.Compilation is CSharpCompilation csharpCompilation))
            {
                throw new Exception($"{nameof(BehaviorAttachedPropertyGenerator)} only support C#.");
            }

            var builder = new StringBuilder();
            foreach (var @class in receiver.CandidateClasses)
            {
                var name = @class.Identifier.ValueText;

                builder.AppendLine($"{name}Property.Changed.Subscribe(OnInstanceChanged);");
                
                context.AddSource($"{name}.g.cs", SourceText.From(GeneratePropertySource(name), Encoding.UTF8));
            }

            context.AddSource($"Acss.SCtor.g.cs", SourceText.From(GenerateSCtorSource(builder.ToString()), Encoding.UTF8));
        }

        private static string GeneratePropertySource(string propertyName)
        {
            const string template = @"
using System;
using Avalonia;

namespace Nlnet.Avalonia.Css.Behaviors;

public partial class Acss
{
    public static AcssBehavior Get$PropertyName$(Visual host)
    {
        return host.GetValue($PropertyName$Property);
    }
    public static void Set$PropertyName$(Visual host, AcssBehavior value)
    {
        host.SetValue($PropertyName$Property, value);
    }
    public static readonly AttachedProperty<AcssBehavior> $PropertyName$Property = AvaloniaProperty
        .RegisterAttached<Acss, Visual, AcssBehavior>(""$PropertyName$"");
}";

            return template.Replace("$PropertyName$", propertyName);
        }

        private static string GenerateSCtorSource(string initialization)
        {
            const string template = @"
using System;
using Avalonia;

namespace Nlnet.Avalonia.Css.Behaviors;

public partial class Acss
{
    static Acss()
    {
        $Initialization$
    }
}";

            return template.Replace("$Initialization$", initialization);
        }
    }
}
