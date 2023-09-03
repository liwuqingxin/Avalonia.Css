using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

internal interface IAcssParser
{
    /// <summary>
    /// Try removing comments like '/* ... */', '//...' and line breaks from span content.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public ReadOnlySpan<char> RemoveComments(Span<char> span);

    /// <summary>
    /// Try parsing imports and relies like 'import ./button.acss;import ./checkbox.acss;rely ./button.acss; ...'.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="imports"></param>
    /// <param name="relies"></param>
    /// <param name="contentSpan"></param>
    public void ParseImportsAndRelies(
        ReadOnlySpan<char> span,
        out IEnumerable<string> imports,
        out IEnumerable<string> relies,
        out ReadOnlySpan<char> contentSpan);
    
    /// <summary>
    /// Try parsing objects like 'definition1 [ setters... ] definition2[setters...]'.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<(string, string)> ParseCollectionObjects(ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing sections like 'selector1 { content... }selector2{ content... }'. It is the same structure as section objects.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<IAcssSection> ParseSections(IAcssSection? parent, ReadOnlySpan<char> span);

    /// <summary>
    /// Try parsing setters and children like 'key:value;... [[ children... ]]'.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="settersSpan"></param>
    /// <param name="childrenSpan"></param>
    /// <returns></returns>
    public void ParseSettersAndChildren(ReadOnlySpan<char> span, out ReadOnlySpan<char> settersSpan, out ReadOnlySpan<char> childrenSpan);

    /// <summary>
    /// Try parsing pairs like 'key:value; key2:value2;...'. Note that string can be wrapped by a '.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public IEnumerable<(string, string)> ParsePairs(ReadOnlySpan<char> span);
}
