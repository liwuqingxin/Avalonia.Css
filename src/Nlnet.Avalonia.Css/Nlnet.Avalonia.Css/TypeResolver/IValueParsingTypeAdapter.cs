using System;

namespace Nlnet.Avalonia.Css;

public interface IValueParsingTypeAdapter
{
    public bool TryAdapt(Type type, out Type? adaptedType);
}
