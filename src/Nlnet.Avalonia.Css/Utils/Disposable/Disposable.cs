// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

//
// -IMPORTANT-
// This is forked from System.Reactive.
// We do not want to reference System.Reactive just for it's Disposable.
// So we forked it.
//

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Provides a set of static methods for creating <see cref="IDisposable"/> objects.
    /// </summary>
    internal static partial class Disposable
    {
        /// <summary>
        /// Represents a disposable that does nothing on disposal.
        /// </summary>
        private sealed class EmptyDisposable : IDisposable
        {
            /// <summary>
            /// Singleton default disposable.
            /// </summary>
            public static readonly EmptyDisposable Instance = new EmptyDisposable();

            private EmptyDisposable()
            {
            }

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Dispose()
            {
                // no op
            }
        }

        /// <summary>
        /// Gets the disposable that does nothing when disposed.
        /// </summary>
        public static IDisposable Empty => EmptyDisposable.Instance;

        /// <summary>
        /// Creates a disposable object that invokes the specified action when disposed.
        /// </summary>
        /// <param name="dispose">Action to run during the first call to <see cref="IDisposable.Dispose"/>. The action is guaranteed to be run at most once.</param>
        /// <returns>The disposable object that runs the given action upon disposal.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dispose"/> is <c>null</c>.</exception>
        public static IDisposable Create(Action dispose)
        {
            if (dispose == null)
            {
                throw new ArgumentNullException(nameof(dispose));
            }

            return new AnonymousDisposable(dispose);
        }

        /// <summary>
        /// Creates a disposable object that invokes the specified action when disposed.
        /// </summary>
        /// <param name="state">The state to be passed to the action.</param>
        /// <param name="dispose">Action to run during the first call to <see cref="IDisposable.Dispose"/>. The action is guaranteed to be run at most once.</param>
        /// <returns>The disposable object that runs the given action upon disposal.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dispose"/> is <c>null</c>.</exception>
        public static IDisposable Create<TState>(TState state, Action<TState> dispose)
        {
            if (dispose == null)
            {
                throw new ArgumentNullException(nameof(dispose));
            }

            return new AnonymousDisposable<TState>(state, dispose);
        }
    }

    internal enum TrySetSingleResult
    {
        Success,
        AlreadyAssigned,
        Disposed
    }

    internal static partial class Disposable
    {
        /// <summary>
        /// Gets the value stored in <paramref name="fieldRef" /> or a null if
        /// <paramref name="fieldRef" /> was already disposed.
        /// </summary>
        internal static IDisposable? GetValue([NotNullIfNotNull("fieldRef")] /*in*/ ref IDisposable? fieldRef)
        {
            var current = Volatile.Read(ref fieldRef);

            return current == BooleanDisposable.True
                ? null
                : current;
        }

        /// <summary>
        /// Gets the value stored in <paramref name="fieldRef" /> or a no-op-Disposable if
        /// <paramref name="fieldRef" /> was already disposed.
        /// </summary>
        [return: NotNullIfNotNull("fieldRef")]
        internal static IDisposable? GetValueOrDefault([NotNullIfNotNull("fieldRef")] /*in*/ ref IDisposable? fieldRef)
        {
            var current = Volatile.Read(ref fieldRef);

            return current == BooleanDisposable.True
                ? EmptyDisposable.Instance
                : current;
        }

        /// <summary>
        /// Tries to assign <paramref name="value" /> to <paramref name="fieldRef" />.
        /// </summary>
        /// <returns>A <see cref="TrySetSingleResult"/> value indicating the outcome of the operation.</returns>
        internal static TrySetSingleResult TrySetSingle([NotNullIfNotNull("value")] ref IDisposable? fieldRef, IDisposable? value)
        {
            var old = Interlocked.CompareExchange(ref fieldRef, value, null);
            if (old == null)
            {
                return TrySetSingleResult.Success;
            }

            if (old != BooleanDisposable.True)
            {
                return TrySetSingleResult.AlreadyAssigned;
            }

            value?.Dispose();
            return TrySetSingleResult.Disposed;
        }

        /// <summary>
        /// Tries to assign <paramref name="value" /> to <paramref name="fieldRef" />. If <paramref name="fieldRef" />
        /// is not disposed and is assigned a different value, it will not be disposed.
        /// </summary>
        /// <returns>true if <paramref name="value" /> was successfully assigned to <paramref name="fieldRef" />.</returns>
        /// <returns>false <paramref name="fieldRef" /> has been disposed.</returns>
        internal static bool TrySetMultiple([NotNullIfNotNull("value")] ref IDisposable? fieldRef, IDisposable? value)
        {
            // Let's read the current value atomically (also prevents reordering).
            var old = Volatile.Read(ref fieldRef);

            for (; ; )
            {
                // If it is the disposed instance, dispose the value.
                if (old == BooleanDisposable.True)
                {
                    value?.Dispose();
                    return false;
                }

                // Atomically swap in the new value and get back the old.
                var b = Interlocked.CompareExchange(ref fieldRef, value, old);

                // If the old and new are the same, the swap was successful and we can quit
                if (old == b)
                {
                    return true;
                }

                // Otherwise, make the old reference the current and retry.
                old = b;
            }
        }

        /// <summary>
        /// Tries to assign <paramref name="value" /> to <paramref name="fieldRef" />. If <paramref name="fieldRef" />
        /// is not disposed and is assigned a different value, it will be disposed.
        /// </summary>
        /// <returns>true if <paramref name="value" /> was successfully assigned to <paramref name="fieldRef" />.</returns>
        /// <returns>false <paramref name="fieldRef" /> has been disposed.</returns>
        internal static bool TrySetSerial([NotNullIfNotNull("value")] ref IDisposable? fieldRef, IDisposable? value)
        {
            var copy = Volatile.Read(ref fieldRef);
            for (; ; )
            {
                if (copy == BooleanDisposable.True)
                {
                    value?.Dispose();
                    return false;
                }

                var current = Interlocked.CompareExchange(ref fieldRef, value, copy);
                if (current == copy)
                {
                    copy?.Dispose();
                    return true;
                }

                copy = current;
            }
        }

        /// <summary>
        /// Disposes <paramref name="fieldRef" />. 
        /// </summary>
        internal static void Dispose([NotNullIfNotNull("fieldRef")] ref IDisposable? fieldRef)
        {
            var old = Interlocked.Exchange(ref fieldRef, BooleanDisposable.True);

            if (old != BooleanDisposable.True)
            {
                old?.Dispose();
            }
        }
    }
}
