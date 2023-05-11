using System;

namespace Nlnet.Avalonia.Css
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class ResourceTypeAttribute : Attribute
    {
        public Type Type { get; set; }

        public ResourceTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
