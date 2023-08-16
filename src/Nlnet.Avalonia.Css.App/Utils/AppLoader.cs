using System;
using System.IO;
using Avalonia.Logging;

namespace Nlnet.Avalonia.Css.App;

internal static class AppLoader
{
    public static void Load(string assemblyName)
    {
        try
        {
            var ext = Path.Combine(Directory.GetCurrentDirectory(), assemblyName);
            System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(ext);
        }
        catch (Exception e)
        {
            Logger.Sink?.Log(LogEventLevel.Error, nameof(AppLoader), null, "{0}", e);
        }
    }
}
