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

        public object? BuildValue(IAcssContext context)
        {
            if (_value != null)
            {
                return _value;
            }
            return ValueString == null ? null : _value = BuildValue(context, ValueString);
        }

        protected abstract object? BuildValue(IAcssContext context, string valueString);

        public object? BuildDeferredValue(IAcssContext context, System.IServiceProvider? provider)
        {
            if (_value != null)
            {
                return _value;
            }
            return ValueString == null ? null : _value = BuildDeferredValueCore(context, provider);
        }

        protected virtual object? BuildDeferredValueCore(IAcssContext context, System.IServiceProvider? provider)
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
