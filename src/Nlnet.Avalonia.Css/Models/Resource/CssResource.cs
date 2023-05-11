using System;

namespace Nlnet.Avalonia.Css
{
    public abstract class CssResource
    {
        public string? Key { get; set; }

        public object? Value { get; set; }

        public bool IsValid => Key != null;

        public bool IsDeferred { get; set; } = false;

        public void AcceptCore(string key, string valueString)
        {
            Key   = key;
            Value = Accept(valueString);
        }

        protected abstract object? Accept(string valueString);
        
        public virtual object? GetDeferredValue(IServiceProvider? provider)
        {
            return null;
        }
    }

    public abstract class CssResource<T> : CssResource, IResourceFactory where T : CssResource, new()
    {
        public CssResource Create()
        {
            return new T();
        }
    }
}
