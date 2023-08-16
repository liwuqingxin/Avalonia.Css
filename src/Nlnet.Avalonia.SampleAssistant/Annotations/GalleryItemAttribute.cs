using System;

namespace Nlnet.Avalonia.SampleAssistant
{
    public enum GalleryItemKind
    {
        Welcome = 0,
        View = 1,
        Demo = 2,
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GalleryItemAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public GalleryItemKind Kind { get; set; }

        public GalleryItemAttribute()
        {
            
        }

        public GalleryItemAttribute(string? name)
        {
            Name = name;
        }

        public GalleryItemAttribute(GalleryItemKind kind, string? name)
        {
            Kind = kind;
            Name = name;
        }

        public GalleryItemAttribute(GalleryItemKind kind, string? name, string? icon)
        {
            Kind = kind;
            Name = name;
            Icon = icon;
        }
    }
}
