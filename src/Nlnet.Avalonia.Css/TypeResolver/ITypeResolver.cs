using System;

namespace Nlnet.Avalonia.Css;

public interface ITypeResolver
{
    public bool TryGetType(string name, out Type? type);
}