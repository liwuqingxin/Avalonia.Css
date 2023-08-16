// ReSharper disable InconsistentNaming

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Controls;

internal static class BBCodeExtension
{
    #region 默认

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string B(this string s)
    {
        return $"[b]{s}[/b]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string I(this string s)
    {
        return $"[i]{s}[/i]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string U(this string s)
    {
        return $"[u]{s}[/u]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Size(this string s, double size)
    {
        return $"[size={size}]{s}[/size]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string C(this string s, string c)
    {
        return $"[color=\"{c}\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Red(this string s)
    {
        return $"[color=\"Red\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Green(this string s)
    {
        return $"[color=\"Green\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DeepSkyBlue(this string s)
    {
        return $"[color=\"DeepSkyBlue\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Orange(this string s)
    {
        return $"[color=\"Orange\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string White(this string s)
    {
        return $"[color=\"White\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Gold(this string s)
    {
        return $"[color=\"Gold\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Pink(this string s)
    {
        return $"[color=\"Pink\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Gray(this string s)
    {
        return $"[color=\"Gray\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DarkGray(this string s)
    {
        return $"[color=\"DarkGray\"]{s}[/color]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LightGray(this string s)
    {
        return $"[color=\"LightGray\"]{s}[/color]";
    }

    #endregion



    #region 替换

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string B(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[b]{target}[/b]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string I(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[i]{target}[/i]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string U(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[u]{target}[/u]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Size(this string s, double size, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[size={size}]{target}[/size]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string C(this string s, string c, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"{c}\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Red(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"Red\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Green(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"Green\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DeepSkyBlue(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"DeepSkyBlue\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Orange(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"Orange\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string White(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"White\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Gold(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"Gold\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Pink(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"Pink\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Gray(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"Gray\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DarkGray(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"DarkGray\"]{target}[/color]{endTag}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LightGray(this string s, string target, string? startTag = null, string? endTag = null)
    {
        return s.Replace(target, $"{startTag}[color=\"LightGray\"]{target}[/color]{endTag}");
    }

    #endregion



    #region 正则

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string B(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[b]{match.Groups[groupIndex].Value}[/b]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string I(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[i]{match.Groups[groupIndex].Value}[/i]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string U(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[u]{match.Groups[groupIndex].Value}[/u]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="size"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Size(this string s, double size, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[size={size}]{match.Groups[groupIndex].Value}[/size]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string C(this string s, string c, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"{c}\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Red(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"Red\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Green(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"Green\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string DeepSkyBlue(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"DeepSkyBlue\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Orange(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"Orange\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string White(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"White\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Gold(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"Gold\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Pink(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"Pink\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string Gray(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"Gray\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string DarkGray(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"DarkGray\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    /// <summary>
    /// You <b>MUST</b> note that replacing by regex is a <b><see langword="DANGEROUS"/></b> operation because you might mismatch the couple of start tag and end tag.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="regex"></param>
    /// <param name="groupIndex"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("You MUST note that replacing by regex is a DANGEROUS operation because you might mismatch the couple of start tag and end tag.")]
    public static string LightGray(this string s, Regex regex, int groupIndex = 0, string? startTag = null, string? endTag = null)
    {
        return regex.Replace(s, match =>
        {
            if (match.Groups.Count < groupIndex)
            {
                groupIndex = 0;
            }
            return $"{startTag}[color=\"LightGray\"]{match.Groups[groupIndex].Value}[/color]{endTag}";
        });
    }

    #endregion
}