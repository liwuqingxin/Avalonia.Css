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
using System.Threading;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Disposable resource with disposal state tracking.
    /// </summary>
    public interface ICancelable : IDisposable
    {
        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        bool IsDisposed { get; }
    }

    /// <summary>Represents an Action-based disposable.</summary>
    internal sealed class AnonymousDisposable : ICancelable, IDisposable
    {
        private volatile Action? _dispose;

        /// <summary>
        /// Constructs a new disposable with the given action used for disposal.
        /// </summary>
        /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
        public AnonymousDisposable(Action dispose) => this._dispose = dispose;

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => this._dispose == null;

        /// <summary>
        /// Calls the disposal action if and only if the current instance hasn't been disposed yet.
        /// </summary>
        public void Dispose()
        {
#pragma warning disable CS8601
#pragma warning disable CS8600
#pragma warning disable CS8625
            var action = Interlocked.Exchange<Action>(ref this._dispose, (Action)null);
#pragma warning restore CS8625
#pragma warning restore CS8600
#pragma warning restore CS8601
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            action?.Invoke();
        }
    }

    /// <summary>
    /// Represents a Action-based disposable that can hold onto some state.
    /// </summary>
    internal sealed class AnonymousDisposable<TState> : ICancelable
    {
        private TState _state;
        private volatile Action<TState>? _dispose;

        /// <summary>
        /// Constructs a new disposable with the given action used for disposal.
        /// </summary>
        /// <param name="state">The state to be passed to the disposal action.</param>
        /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
        public AnonymousDisposable(TState state, Action<TState> dispose)
        {
            //Diagnostics.Debug.Assert(dispose != null);

            _state = state;
            _dispose = dispose;
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => _dispose == null;

        /// <summary>
        /// Calls the disposal action if and only if the current instance hasn't been disposed yet.
        /// </summary>
        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke(_state);
            _state = default!;
        }
    }
}