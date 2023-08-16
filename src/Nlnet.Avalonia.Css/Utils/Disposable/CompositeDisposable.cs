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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
#pragma warning disable CS8603
#pragma warning disable CS8625
#pragma warning disable CS8600
#pragma warning disable CS8619

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Represents a group of disposable resources that are disposed together.
    /// </summary>
    internal sealed class CompositeDisposable :
      ICollection<IDisposable>,
      IEnumerable<IDisposable>,
      IEnumerable,
      ICancelable,
      IDisposable
    {
        private readonly object _gate = new();
        private bool _disposed;
        private List<IDisposable?> _disposables;
        private int _count;

        private const int ShrinkThreshold = 64;
        private const int DefaultCapacity = 16;

        /// <summary>
        /// An empty enumerator for the <see cref="M:Nlnet.Avalonia.Css.CompositeDisposable.GetEnumerator" />
        /// method to avoid allocation on disposed or empty composites.
        /// </summary>
        private static readonly CompositeDisposable.CompositeEnumerator EmptyEnumerator = new(Array.Empty<IDisposable>());

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> class with no disposables contained by it initially.
        /// </summary>
        public CompositeDisposable() => this._disposables = new List<IDisposable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> class with the specified number of disposables.
        /// </summary>
        /// <param name="capacity">The number of disposables that the new CompositeDisposable can initially store.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity" /> is less than zero.</exception>
        public CompositeDisposable(int capacity) => this._disposables = capacity >= 0 ? new List<IDisposable>(capacity) : throw new ArgumentOutOfRangeException(nameof(capacity));

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> class from a group of disposables.
        /// </summary>
        /// <param name="disposables">Disposables that will be disposed together.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="disposables" /> is <c>null</c>.</exception>
        /// <exception cref="T:System.ArgumentException">Any of the disposables in the <paramref name="disposables" /> collection is <c>null</c>.</exception>
        public CompositeDisposable(params IDisposable[] disposables)
        {
            this._disposables = disposables != null ? CompositeDisposable.ToList((IEnumerable<IDisposable>)disposables) : throw new ArgumentNullException(nameof(disposables));
            Volatile.Write(ref this._count, this._disposables.Count);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> class from a group of disposables.
        /// </summary>
        /// <param name="disposables">Disposables that will be disposed together.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="disposables" /> is <c>null</c>.</exception>
        /// <exception cref="T:System.ArgumentException">Any of the disposables in the <paramref name="disposables" /> collection is <c>null</c>.</exception>
        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            this._disposables = disposables != null ? CompositeDisposable.ToList(disposables) : throw new ArgumentNullException(nameof(disposables));
            Volatile.Write(ref this._count, this._disposables.Count);
        }

        private static List<IDisposable?> ToList(IEnumerable<IDisposable> disposables)
        {
            int capacity;
            switch (disposables)
            {
                case IDisposable[] disposableArray:
                    capacity = disposableArray.Length;
                    break;
                case ICollection<IDisposable> disposables1:
                    capacity = disposables1.Count;
                    break;
                default:
                    capacity = 16;
                    break;
            }
            List<IDisposable> list = new List<IDisposable>(capacity);
            foreach (IDisposable disposable in disposables)
            {
                if (disposable == null)
                    throw new ArgumentException("Strings_Core.DISPOSABLES_CANT_CONTAIN_NULL", nameof(disposables));
                list.Add(disposable);
            }
            return list;
        }

        /// <summary>
        /// Gets the number of disposables contained in the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" />.
        /// </summary>
        public int Count => Volatile.Read(ref this._count);

        /// <summary>
        /// Adds a disposable to the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> or disposes the disposable if the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> is disposed.
        /// </summary>
        /// <param name="item">Disposable to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="item" /> is <c>null</c>.</exception>
        public void Add(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            lock (this._gate)
            {
                if (!this._disposed)
                {
                    this._disposables.Add(item);
                    Volatile.Write(ref this._count, this._count + 1);
                    return;
                }
            }
            item.Dispose();
        }

        /// <summary>
        /// Removes and disposes the first occurrence of a disposable from the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" />.
        /// </summary>
        /// <param name="item">Disposable to remove.</param>
        /// <returns>true if found; false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="item" /> is <c>null</c>.</exception>
        public bool Remove(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            lock (this._gate)
            {
                if (this._disposed)
                    return false;
                List<IDisposable> disposables = this._disposables;
                int index = disposables.IndexOf(item);
                if (index < 0)
                    return false;
                disposables[index] = (IDisposable)null;
                if (disposables.Capacity > 64 && this._count < disposables.Capacity / 2)
                {
                    List<IDisposable> disposableList = new List<IDisposable>(disposables.Capacity / 2);
                    foreach (IDisposable disposable in disposables)
                    {
                        if (disposable != null)
                            disposableList.Add(disposable);
                    }
                    this._disposables = disposableList;
                }
                Volatile.Write(ref this._count, this._count - 1);
            }
            item.Dispose();
            return true;
        }

        /// <summary>
        /// Disposes all disposables in the group and removes them from the group.
        /// </summary>
        public void Dispose()
        {
            List<IDisposable> disposableList = (List<IDisposable>)null;
            lock (this._gate)
            {
                if (!this._disposed)
                {
                    disposableList = this._disposables;
                    this._disposables = (List<IDisposable>)null;
                    Volatile.Write(ref this._count, 0);
                    Volatile.Write(ref this._disposed, true);
                }
            }
            if (disposableList == null)
                return;
            foreach (IDisposable disposable in disposableList)
                disposable?.Dispose();
        }

        /// <summary>
        /// Removes and disposes all disposables from the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" />, but does not dispose the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" />.
        /// </summary>
        public void Clear()
        {
            IDisposable[] array;
            lock (this._gate)
            {
                if (this._disposed)
                    return;
                List<IDisposable> disposables = this._disposables;
                array = disposables.ToArray();
                disposables.Clear();
                Volatile.Write(ref this._count, 0);
            }
            foreach (IDisposable disposable in array)
                disposable?.Dispose();
        }

        /// <summary>
        /// Determines whether the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> contains a specific disposable.
        /// </summary>
        /// <param name="item">Disposable to search for.</param>
        /// <returns>true if the disposable was found; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="item" /> is <c>null</c>.</exception>
        public bool Contains(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            lock (this._gate)
                return !this._disposed && this._disposables.Contains(item);
        }

        /// <summary>
        /// Copies the disposables contained in the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" /> to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">Array to copy the contained disposables to.</param>
        /// <param name="arrayIndex">Target index at which to copy the first disposable of the group.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is <c>null</c>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than zero. -or - <paramref name="arrayIndex" /> is larger than or equal to the array length.</exception>
        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            lock (this._gate)
            {
                if (this._disposed)
                    return;
                if (arrayIndex + this._count > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                int num = arrayIndex;
                foreach (IDisposable disposable in this._disposables)
                {
                    if (disposable != null)
                        array[num++] = disposable;
                }
            }
        }

        /// <summary>Always returns false.</summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" />.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        public IEnumerator<IDisposable> GetEnumerator()
        {
            lock (this._gate)
                return this._disposed || this._count == 0 ? (IEnumerator<IDisposable>)CompositeDisposable.EmptyEnumerator : (IEnumerator<IDisposable>)new CompositeDisposable.CompositeEnumerator(this._disposables.ToArray());
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:Nlnet.Avalonia.Css.CompositeDisposable" />.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => Volatile.Read(ref this._disposed);

        /// <summary>An enumerator for an array of disposables.</summary>
        private sealed class CompositeEnumerator : IEnumerator<IDisposable>, IEnumerator, IDisposable
        {
            private readonly IDisposable?[] _disposables;
            private int _index;

            public CompositeEnumerator(IDisposable?[] disposables)
            {
                this._disposables = disposables;
                this._index = -1;
            }

            public IDisposable Current => this._disposables[this._index];

            object IEnumerator.Current => (object)this._disposables[this._index];

            public void Dispose()
            {
                IDisposable[] disposables = this._disposables;
                Array.Clear((Array)disposables, 0, disposables.Length);
            }

            public bool MoveNext()
            {
                IDisposable[] disposables = this._disposables;
                int index;
                do
                {
                    index = ++this._index;
                    if (index >= disposables.Length)
                        return false;
                }
                while (disposables[index] == null);
                return true;
            }

            public void Reset() => this._index = -1;
        }
    }
}
