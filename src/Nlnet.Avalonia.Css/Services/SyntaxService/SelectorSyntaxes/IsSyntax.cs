﻿using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class IsSyntax : ISyntax, ITypeSyntax
{
    public string TypeName { get; set; } = string.Empty;

    public string Xmlns { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        var manager = context.GetService<ITypeResolverManager>();
        if (manager.TryGetType(TypeName, out var type))
        {
            return previous.Is(type!);
        }

        this.WriteError($"Can not resolve the type '{TypeName}'.");
        return previous;
    }
}
