using System.Diagnostics;

namespace Nlnet.Avalonia.Css
{
    internal static class DiagnosisHelper
    {
        private static readonly string? Assembly;

        static DiagnosisHelper()
        {
            Assembly = typeof(DiagnosisHelper).Assembly.GetName().Name;
        }

        [Conditional("DEBUG")]
        public static void WriteLine(this object owner, string text)
        {
            Trace.WriteLine($"[{Assembly}] ({owner}): {text}");
        }
    }
}
