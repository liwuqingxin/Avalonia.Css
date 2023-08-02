using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

internal interface IAcssParser
{
    /// <summary>
    /// Try parsing section objects like 'definition { setters...}'.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<(string, string)> ParseObjects(ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing sections like 'selector { content... }'. It is the same structure as section objects.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<IAcssSection> ParseSections(IAcssSection? parent, ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing pairs like 'key:value; key2:value2...'. Note that string can be wrapped by a '.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<(string, string)> ParsePairs(ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing setters and children like 'key:value;... [[ children... ]]'.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="settersSpan"></param>
    /// <param name="childrenSpan"></param>
    /// <returns></returns>
    public void ParseSettersAndChildren(ReadOnlySpan<char> span, out ReadOnlySpan<char> settersSpan, out ReadOnlySpan<char> childrenSpan);

    /// <summary>
    /// Try removing comments like '/* ... */' from span content.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public ReadOnlySpan<char> RemoveComments(Span<char> span);

}
