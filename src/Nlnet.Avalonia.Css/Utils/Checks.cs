using System;
using Avalonia;

namespace Nlnet.Avalonia.Css
{
    internal static class Checks
    {
        /// <summary>
        /// Check and return the <see cref="Application"/>.<see cref="Application.Current"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Application CheckApplication()
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException($"Application.Current is null.");
            }

            return Application.Current;
        }
    }
}
