using System;

namespace Nlnet.Avalonia.Css
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class ResourceTypeAttribute : Attribute
    {
        public string Type { get; set; }

        public ResourceTypeAttribute(string type)
        {
            Type = type;
        }
    }
}
