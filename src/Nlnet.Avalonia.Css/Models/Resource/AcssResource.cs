using System;

namespace Nlnet.Avalonia.Css
{
    internal abstract class AcssResource
    {
        private object? _value;

        public string? Key { get; set; }

        public string? ValueString { get; set; }

        public bool IsValid => Key != null;

        public bool IsDeferred { get; set; } = false;

        public void Accept(string key, string valueString)
        {
            Key         = key;
            ValueString = valueString;
        }

        public object? BuildValue(IAcssBuilder acssBuilder)
        {
            if (_value != null)
            {
                return _value;
            }
            return ValueString == null ? null : _value = BuildValue(acssBuilder, ValueString);
        }

        protected abstract object? BuildValue(IAcssBuilder acssBuilder, string valueString);

        public object? BuildDeferredValue(IAcssBuilder acssBuilder, IServiceProvider? provider)
        {
            if (_value != null)
            {
                return _value;
            }
            return ValueString == null ? null : _value = BuildDeferredValueCore(acssBuilder, provider);
        }

        protected virtual object? BuildDeferredValueCore(IAcssBuilder acssBuilder, IServiceProvider? provider)
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
