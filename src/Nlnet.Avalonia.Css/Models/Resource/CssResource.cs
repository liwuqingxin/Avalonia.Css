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

    public interface IResourceFactory
    {
        public CssResource Create();
    }


    public abstract class CssResourceBaseAndFac<T> : CssResource, IResourceFactory where T : CssResource, new()
    {
        CssResource IResourceFactory.Create()
        {
            return new T();
        }
    }
}
