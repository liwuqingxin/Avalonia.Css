using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Utilities;

namespace Nlnet.Avalonia.Css.App
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs Avalonia events to the <see cref="System.Diagnostics.Trace"/> sink.
        /// </summary>
        /// <typeparam name="T">The application class type.</typeparam>
        /// <param name="builder">The app builder instance.</param>
        /// <param name="level">The minimum level to log.</param>
        /// <param name="areas">The areas to log. Valid values are listed in <see cref="LogArea"/>.</param>
        /// <returns>The app builder instance.</returns>
        public static AppBuilder LogToLocalFile(
            this AppBuilder builder,
            LogEventLevel   level = LogEventLevel.Warning,
            params string[] areas)
        {
            Logger.Sink = new LogSink(level, areas);
            return builder;
        }
    }

    internal class LogSink : ILogSink
    {
        private readonly LogEventLevel  _level;
        private readonly IList<string>? _areas;
        private readonly string         _path = $"./logs/{DateTime.Today:yyyy-M-d dddd}.log";

        public LogSink(LogEventLevel minimumLevel, IList<string>? areas = null)
        {
            this._level = minimumLevel;
            this._areas = areas is not {Count: > 0} ? null : areas;

            var dir = Path.GetDirectoryName(this._path);
            if (string.IsNullOrEmpty(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
        }

        public bool IsEnabled(LogEventLevel level, string area)
        {
            if (level < this._level)
            {
                return false;
            }

            var areas = this._areas;
            return areas == null || areas.Contains(area);
        }

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
        {
            if (!this.IsEnabled(level, area))
            {
                return;
            }

            var logMessage = LogSink.Format<object, object, object>(area, messageTemplate, source, null);

            var stream = File.AppendText(_path);
            stream.WriteLine(logMessage);
            stream.Close();
        }

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
        {
            if (!this.IsEnabled(level, area))
            {
                return;
            }
            var logMessage = LogSink.Format(area, messageTemplate, source, propertyValues);

            var stream = File.AppendText(_path);
            stream.WriteLine(logMessage);
            stream.Close();
        }



        private static string Format<T0, T1, T2>(
            string                  area,
            string                  template,
            object?                 source,
            IReadOnlyList<object?>? values)
        {
            var sb = StringBuilderCache.Acquire(template.Length);
            var characterReader = new CharacterReader(template.AsSpan());
            var num1 = 0;
            sb.Append('[');
            sb.Append(area);
            sb.Append("] ");
            while (!characterReader.End)
            {
                var ch = characterReader.Take();
                if (ch != '{')
                    sb.Append(ch);
                else if (characterReader.Peek != '{')
                {
                    sb.Append('\'');
                    sb.Append(values?[num1++]);
                    sb.Append('\'');
                    characterReader.TakeUntil('}');
                    var num2 = (int) characterReader.Take();
                }
                else
                {
                    sb.Append('{');
                    var num3 = (int) characterReader.Take();
                }
            }
            if (source != null)
            {
                sb.Append(" (");
                sb.Append(source.GetType().Name);
                sb.Append(" #");
                sb.Append(source.GetHashCode());
                sb.Append(')');
            }
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private static string Format(string area, string template, object? source, IReadOnlyList<object?> v)
        {
            var stringBuilder = StringBuilderCache.Acquire(template.Length);
            var characterReader = new CharacterReader(template.AsSpan());
            var num1 = 0;
            stringBuilder.Append('[');
            stringBuilder.Append(area);
            stringBuilder.Append(']');
            while (!characterReader.End)
            {
                var ch = characterReader.Take();
                if (ch != '{')
                    stringBuilder.Append(ch);
                else if (characterReader.Peek != '{')
                {
                    stringBuilder.Append('\'');
                    stringBuilder.Append(num1 < v.Count ? v[num1++] : null);
                    stringBuilder.Append('\'');
                    characterReader.TakeUntil('}');
                    var num2 = (int) characterReader.Take();
                }
                else
                {
                    stringBuilder.Append('{');
                    var num3 = (int) characterReader.Take();
                }
            }
            if (source != null)
            {
                stringBuilder.Append('(');
                stringBuilder.Append(source.GetType().Name);
                stringBuilder.Append(" #");
                stringBuilder.Append(source.GetHashCode());
                stringBuilder.Append(')');
            }
            return stringBuilder.ToString();
        }
    }

    internal static class StringBuilderCache
    {
        internal const int MaxBuilderSize  = 360;
        private const  int DefaultCapacity = 16;
        [ThreadStatic]
        private static StringBuilder? _tCachedInstance;

        /// <summary>Get a StringBuilder for the specified capacity.</summary>
        /// <remarks>If a StringBuilder of an appropriate size is cached, it will be returned and the cache emptied.</remarks>
        public static StringBuilder Acquire(int capacity = 16)
        {
            if (capacity <= 360)
            {
                var tCachedInstance = StringBuilderCache._tCachedInstance;
                if (tCachedInstance != null && capacity <= tCachedInstance.Capacity)
                {
                    StringBuilderCache._tCachedInstance = null;
                    tCachedInstance.Clear();
                    return tCachedInstance;
                }
            }
            return new StringBuilder(capacity);
        }

        /// <summary>Place the specified builder in the cache if it is not too big.</summary>
        private static void Release(StringBuilder sb)
        {
            if (sb.Capacity > 360)
                return;
            StringBuilderCache._tCachedInstance = sb;
        }

        /// <summary>ToString() the stringbuilder, Release it to the cache, and return the resulting string.</summary>
        public static string GetStringAndRelease(StringBuilder sb)
        {
            var stringAndRelease = sb.ToString();
            StringBuilderCache.Release(sb);
            return stringAndRelease;
        }
    }
}
