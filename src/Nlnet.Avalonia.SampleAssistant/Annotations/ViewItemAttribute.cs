using System;

namespace Nlnet.Avalonia.SampleAssistant
{
    public enum ViewItemKind
    {
        Welcome = 0,
        View = 1,
        Demo = 2,
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ViewItemAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public ViewItemKind Kind { get; set; }

        public ViewItemAttribute()
        {
            
        }

        public ViewItemAttribute(string? name)
        {
            Name = name;
        }

        public ViewItemAttribute(ViewItemKind kind, string? name)
        {
            Kind = kind;
            Name = name;
        }

        public ViewItemAttribute(ViewItemKind kind, string? name, string? icon)
        {
            Kind = kind;
            Name = name;
            Icon = icon;
        }
    }
}
