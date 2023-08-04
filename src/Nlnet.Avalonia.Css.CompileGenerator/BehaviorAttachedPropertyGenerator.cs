using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            private static readonly Regex Regex = new Regex("typeof\\((.*)\\)");

            public Dictionary<string, List<ClassDeclarationSyntax>> CandidateClasses { get; } = new Dictionary<string, List<ClassDeclarationSyntax>>();
            
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (!(syntaxNode is ClassDeclarationSyntax classDeclarationSyntax) || classDeclarationSyntax.AttributeLists.Count <= 0)
                {
                    return;
                }

                var isBehavior = false;
                var ownerType  = "";

                foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
                {
                    var attr = attributeListSyntax
                               .Attributes
                               .FirstOrDefault(a => a.Name.ToFullString() == "Behavior");

                    ownerType = attr?.ArgumentList?.Arguments[1].Expression.ToFullString();
                    if (ownerType == null)
                    {
                        continue;
                    }
                    var match = Regex.Match(ownerType);
                    if (match.Success == false)
                    {
                        continue;
                    }

                    ownerType  = match.Groups[1].Value;
                    isBehavior = true;

                    break;
                }

                if (isBehavior == false || string.IsNullOrEmpty(ownerType))
                {
                    return;
                }

                if (CandidateClasses.TryGetValue(ownerType, out var list))
                {
                    list.Add(classDeclarationSyntax);
                }
                else
                {
                    list = new List<ClassDeclarationSyntax> { classDeclarationSyntax };
                    CandidateClasses[ownerType] = list;
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
            foreach (var pair in receiver.CandidateClasses)
            {
                foreach (var name in pair.Value.Select(syntax => syntax.Identifier.ValueText))
                {
                    builder.AppendLine($"{name}Property.Changed.Subscribe(AcssBehaviorHelper.OnInstanceChanged);");

                    var source = GeneratePropertySource(null, pair.Key, name);
                    context.AddSource($"{name}.g.cs", SourceText.From(source, Encoding.UTF8));
                }

                var ctorSource = GenerateSCtorSource(null, pair.Key, builder.ToString());
                context.AddSource($"{pair.Key}.ctor.g.cs", SourceText.From(ctorSource, Encoding.UTF8));
            }
        }

        private static string GeneratePropertySource(string importNamespace, string className, string propertyName)
        {
            const string template = @"
using System;
using Avalonia;
using Nlnet.Avalonia.Css;
$Namespace$

namespace Nlnet.Avalonia.Css.Behaviors;

public partial class $ClassName$
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
        .RegisterAttached<$ClassName$, Visual, AcssBehavior>(""$PropertyName$"");
}";

            return template
                   .Replace("$Namespace$", importNamespace)
                   .Replace("$ClassName$", className)
                   .Replace("$PropertyName$", propertyName);
        }

        private static string GenerateSCtorSource(string importNamespace, string className, string initialization)
        {
            const string template = @"
using System;
using Avalonia;
using Nlnet.Avalonia.Css;
$Namespace$

namespace Nlnet.Avalonia.Css.Behaviors;

public partial class $ClassName$
{
    static $ClassName$()
    {
        $Initialization$
    }
}";
            
            return template
                   .Replace("$Namespace$", importNamespace)
                   .Replace("$ClassName$", className)
                   .Replace("$Initialization$", initialization);
        }
    }
}
