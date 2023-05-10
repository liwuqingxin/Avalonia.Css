using System.IO;

namespace Nlnet.Avalonia.Css.App.Utils;

internal static class AppLoader
{
    public static void Load(string assemblyName)
    {
        var ext = Path.Combine(Directory.GetCurrentDirectory(), assemblyName);
        System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(ext);
    }
}
