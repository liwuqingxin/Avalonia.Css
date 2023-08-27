using System.IO;

namespace Nlnet.Avalonia.Css;

public static class PathHelper
{
    public static string GetStandardPath(this string path)
    {
        // Path is case sensitive in linux.
        return Path.GetFullPath(path)/*.ToLower()*/;
    }
}