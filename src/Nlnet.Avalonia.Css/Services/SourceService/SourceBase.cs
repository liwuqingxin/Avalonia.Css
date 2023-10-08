using System;

namespace Nlnet.Avalonia.Css;

public abstract class SourceBase : ISource, IEquatable<SourceBase>
{
    public event EventHandler<EventArgs>? SourceChanged;

    void ISource.OnSourceChanged()
    {
        SourceChanged?.Invoke(this, EventArgs.Empty);
    }

    public abstract string GetKeyPath();

    public abstract string? GetSource();

    public abstract bool IsValid();

    public abstract ISource CreateFromPath(string path);



    public override bool Equals(object? obj)
    {
        return obj is ISource source && Equals(GetKeyPath(), source.GetKeyPath());
    }

    bool IEquatable<SourceBase>.Equals(SourceBase? other)
    {
        return this.Equals(other);
    }

    public override int GetHashCode()
    {
        return GetKeyPath().GetHashCode();
    }

    public override string ToString()
    {
        return $"{this.GetType()} : {GetKeyPath()}";
    }
}