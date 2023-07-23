using System.Diagnostics;

namespace Nlnet.Avalonia.Css
{
    public static class DiagnosisHelper
    {
        private static readonly string? Assembly;

        static DiagnosisHelper()
        {
            Assembly = typeof(DiagnosisHelper).Assembly.GetName().Name;
        }

        [Conditional("TRACE")]
        public static void WriteLine(string text)
        {
            Trace.WriteLine($"[{Assembly}] {text}");
        }

        [Conditional("TRACE")]
        public static void WriteLine(this object owner, string text)
        {
            Trace.WriteLine($"[{Assembly}] ({owner}): {text}");
        }

        [Conditional("TRACE")]
        public static void WriteError(this object owner, string text)
        {
            Trace.WriteLine($"[{Assembly}] ({owner}): [Error] {text}");
        }

        [Conditional("TRACE")]
        public static void WriteWarning(this object owner, string text)
        {
            Trace.WriteLine($"[{Assembly}] ({owner}): [Warning] {text}");
        }
    }
}
