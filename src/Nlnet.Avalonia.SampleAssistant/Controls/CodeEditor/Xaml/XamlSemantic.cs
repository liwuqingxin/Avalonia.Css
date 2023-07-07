using System;

namespace Nlnet.Avalonia.SampleAssistant;

[Flags]
public enum XamlSemantic : ulong
{
    None = 0,
    Element = 1,
    Attribute = 1 << 1,
    Value = 1 << 2,
    StringStart = 1 << 3,
    StringEnd = 1 << 4,
    Namespace = 1 << 5,
    Comment = 1 << 6,
    Prefix = 1 << 7,
    Markup = 1 << 8,
}
