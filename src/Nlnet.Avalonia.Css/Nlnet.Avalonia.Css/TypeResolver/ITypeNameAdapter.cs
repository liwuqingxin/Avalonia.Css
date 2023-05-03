namespace Nlnet.Avalonia.Css;

public interface ITypeNameAdapter
{
    public bool TryAdapt(string name, out string? adaptedName);
}
