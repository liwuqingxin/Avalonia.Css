using System;

namespace Nlnet.Avalonia.Css
{
    public interface ISource
    {
        public event EventHandler<EventArgs> SourceChanged;

        public string GetKeyPath();

        public string? GetSource();

        ISource CreateFromPath(string path);

        public void OnSourceChanged();
    }

    public abstract class SourceBase : ISource, IEquatable<SourceBase>
    {
        public event EventHandler<EventArgs>? SourceChanged;

        public abstract string GetKeyPath();

        public abstract string? GetSource();

        public abstract ISource CreateFromPath(string path);

        void ISource.OnSourceChanged()
        {
            SourceChanged?.Invoke(this, EventArgs.Empty);
        }



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
    }
}
