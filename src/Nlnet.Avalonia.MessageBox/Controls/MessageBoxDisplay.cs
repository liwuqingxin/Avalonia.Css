using System.Globalization;

namespace Nlnet.Avalonia.Controls;

public static class MessageBoxDisplay
{
    public static string? Ok => Get("Ok");

    public static string? Yes => Get("Yes");
    
    public static string? No => Get("No");
    
    public static string? Cancel => Get("Cancel");

    
    
    private static string? Get(string text)
    {
        var culture = CultureInfo.CurrentCulture;
        return culture.Name switch
        {
            "zh-CN" => Cn(text),
            _ => En(text)
        };
    }
    
    private static string? Cn(string parameter)
    {
        return parameter switch
        {
            "Ok" => "确定",
            "Yes" => "是",
            "No" => "否",
            "Cancel" => "取消",
            _ => null
        };
    }

    private static string? En(string parameter)
    {
        return parameter;
    }
}