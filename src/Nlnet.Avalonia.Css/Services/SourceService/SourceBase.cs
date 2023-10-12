using System;

namespace Nlnet.Avalonia.Css;

public abstract class SourceBase<TKey> : ISource, IEquatable<SourceBase<TKey>>
{
    public event EventHandler<EventArgs>? SourceChanged;

    public void OnSourceChanged()
    {
        SourceChanged?.Invoke(this, EventArgs.Empty);
    }

    public abstract TKey GetKey();

    public abstract string? GetSource();

    public abstract bool IsValid();

    public abstract ISource CreateFromPath(string path, bool alignPathToThis);

    public abstract void Attach(IAcssContext context);

    public abstract void Detach(IAcssContext context);


    public override bool Equals(object? obj)
    {
        return obj is SourceBase<TKey> source && Equals(GetKey(), source.GetKey());
    }

    bool IEquatable<SourceBase<TKey>>.Equals(SourceBase<TKey>? other)
    {
        return this.Equals(other);
    }

    public override int GetHashCode()
    {
        return GetKey()?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return $"{this.GetType()} : {GetKey()}";
    }
}