namespace Nlnet.Avalonia.Css;

// ReSharper disable InconsistentNaming

public enum AcssErrors
{
    None = 0,
    Exception = 1,

    Control_Theme_Not_Found,
    
    Source_Invalid,
    Value_String_Invalid,

    Animation_With_Invalid_Parent,
    Animation_With_Invalid_Target_Type,
    Animation_With_Empty_Frames,

    ResourceDictionary_Empty,
    ResourceDictionary_Accent_Not_Match,
    
    Resource_Invalid,
    Resource_Not_Supported,
}