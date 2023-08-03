using System;

namespace Nlnet.Avalonia.Css
{
    internal abstract class AcssResource
    {
        public string? Key { get; set; }

        public object? Value { get; set; }

        public bool IsValid => Key != null;

        public bool IsDeferred { get; set; } = false;

        public void AcceptCore(IAcssBuilder cssBuilder, string key, string valueString)
        {
            Key   = key;
            Value = Accept(cssBuilder, valueString);
        }

        protected abstract object? Accept(IAcssBuilder acssBuilder, string valueString);
        
        public virtual object? GetDeferredValue(IServiceProvider? provider)
        {
            return null;
        }
    }

    internal interface IResourceFactory
    {
        public AcssResource Create();
    }


    internal abstract class AcssResourceBaseAndFac<T> : AcssResource, IResourceFactory where T : AcssResource, new()
    {
        AcssResource IResourceFactory.Create()
        {
            return new T();
        }
    }
}
