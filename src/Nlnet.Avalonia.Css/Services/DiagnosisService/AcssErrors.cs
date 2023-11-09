namespace Nlnet.Avalonia.Css;

// ReSharper disable InconsistentNaming

public enum AcssErrors
{
    None = 0,
    Exception = 1,

    ControlTheme_Not_Found,
    
    Source_Invalid,

    Value_String_Invalid,
    Value_String_Null,

    Animation_Parent_Invalid,
    Animation_TargetType_Not_Found,
    Animation_Empty,

    ResourceDictionary_Empty,
    ResourceDictionary_Accent_Not_Match,
    
    Resource_Invalid,
    Resource_Not_Found,
    Resource_Not_Supported,

    Setter_Duplicated,

    Style_TargetType_Not_Found,

    Property_Not_Found,
    Property_String_Invalid,

    Type_Not_Found,

    Selector_Invalid,

    Transition_Not_Found,
}