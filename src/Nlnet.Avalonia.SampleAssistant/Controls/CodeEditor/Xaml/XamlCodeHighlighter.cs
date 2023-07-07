using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;
using Nlnet.Avalonia.Css.App.Controls;

namespace Nlnet.Avalonia.SampleAssistant;

public class XamlCodeHighlighter : ICodeHighlighter
{
    private readonly CodeStyleContext<XamlSemantic> _context;

    private readonly TextRunProperties _default;
    private readonly TextRunProperties _element;
    private readonly TextRunProperties _attribute;
    private readonly TextRunProperties _markup;
    private readonly TextRunProperties _string;
    private readonly TextRunProperties _symbol;
    private readonly TextRunProperties _equalSign;
    private readonly TextRunProperties _comment;

    private readonly TextRunProperties _gentle;
    private readonly TextRunProperties _salient;
    private readonly TextRunProperties _striking;

    private readonly Reference<TextRunProperties?> _currentRef = new();
    private readonly List<ValueSpan<TextRunProperties>> _styles = new();

    private int _startIndex = 0;



    public XamlCodeHighlighter(CodeStyleContext<XamlSemantic> context)
    {
        var bold = new Typeface(context.Typeface.FontFamily, context.Typeface.Style, FontWeight.Bold, context.Typeface.Stretch);

        _context = context;
        _default = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: context.Foreground);
        _element = new GenericTextRunProperties(bold, context.FontSize, foregroundBrush: Brushes.Coral);
        _attribute = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.BlueViolet);
        _markup = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.DarkOliveGreen);
        _string = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.CadetBlue);
        _symbol = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.Orange);
        _equalSign = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.Green);
        _comment = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.DarkGreen);
        _gentle = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.SkyBlue);
        _salient = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.Gray);
        _striking = new GenericTextRunProperties(context.Typeface, context.FontSize, foregroundBrush: Brushes.Orange);
    }

    public bool TryHighlight(string? code, ICodeStyleContext context, out IReadOnlyList<ValueSpan<TextRunProperties>>? styles)
    {
        if (string.IsNullOrEmpty(code))
        {
            styles = null;
            return false;
        }

        for (var i = 0; i < code.Length; i++)
        {
            var ch = code[i];
            HandleChar(ch, i);
        }

        styles = _styles;

        return true;
    }

    private void HandleChar(char ch, int index)
    {
        switch (ch)
        {
            case '<':
            case '>':
            case '=':
            case '"':
            case '.':
            case ':':
            case '{':
            case '}':
            case '/':
            case '|':
            case '#':
            case '$':
            case '&':
                PushCurrentStyle(index);
                PushSingleCharStyle(ch, index);
                StartAtNext(index);
                break;
            case ' ':
            case '\t':
            case '\n':
                break;
            case '\r':
                break;
            default:
                break;
        }

        switch (ch)
        {
            case '<':
                if (CheckSemantic(XamlSemantic.StringStart) == false)
                {
                    AddSemantic(XamlSemantic.Element);
                    if (CheckCharBeforeChar(index, ':', ' ', '\n', '\t'))
                    {
                        AddSemantic(XamlSemantic.Prefix);
                        SetCurrentStyle(_salient);
                    }
                    else
                    {
                        SetCurrentStyle(_element);
                    }
                }
                break;
            case '>':
                if (CheckSemantic(XamlSemantic.Comment) && CheckChar(index, -1, '-') && CheckChar(index, -2, '-'))
                {
                    PopupStyle();
                    PushCurrentStyle(index);
                    RemoveSemantic(XamlSemantic.Comment);
                    AddSemanticWhenNo(XamlSemantic.None, XamlSemantic.StringStart, XamlSemantic.Comment);
                    SetCurrentStyle(_default);
                }
                else
                {
                    RemoveSemantic(XamlSemantic.Element);
                    AddSemanticWhenNo(XamlSemantic.None, XamlSemantic.StringStart, XamlSemantic.Comment);
                    SetCurrentStyle(_default);
                }
                break;
            case '=':
                AddSemanticWhenNo(XamlSemantic.Value, XamlSemantic.StringStart, XamlSemantic.Comment);
                break;
            case '"':
                if (CheckSemantic(XamlSemantic.Comment) == false)
                {
                    if (_context.Semantics.HasFlag(XamlSemantic.StringEnd) || _context.Semantics.HasFlag(XamlSemantic.StringStart) == false)
                    {
                        AddSemantic(XamlSemantic.StringStart);
                        RemoveSemantic(XamlSemantic.StringEnd);
                        SetCurrentStyle(_string);
                    }
                    else
                    {
                        AddSemantic(XamlSemantic.StringEnd);
                        RemoveSemantic(XamlSemantic.StringStart);
                        RemoveSemantic(XamlSemantic.Value);
                        SetCurrentStyle(_default);
                    }
                }
                break;
            case '!':
                if (CheckChar(index, -1, '<') &&
                    CheckChar(index, 1, '-') &&
                    CheckChar(index, 2, '-') &&
                    CheckSemantic(XamlSemantic.Element) &&
                    CheckSemantic(XamlSemantic.StringStart) == false)
                {
                    PopupStyle();
                    MoveStart(-1);
                    AddSemantic(XamlSemantic.Comment);
                    RemoveSemantic(XamlSemantic.Element);
                    SetCurrentStyle(_comment);
                }
                break;
            case ':':
                if (CheckSemantic(XamlSemantic.Element) && CheckSemantic(XamlSemantic.Prefix))
                {
                    RemoveSemantic(XamlSemantic.Prefix);
                    SetCurrentStyle(_element);
                }
                break;
            case '{':
                if (CheckSemantic(XamlSemantic.StringStart))
                {
                    AddSemantic(XamlSemantic.Markup);
                    SetCurrentStyle(_markup);
                }
                break;
            case '}':
                if (CheckSemantic(XamlSemantic.Markup))
                {
                    RemoveSemantic(XamlSemantic.Markup);
                    SetCurrentStyle(_default);
                }
                break;
            case '/':
            case '|':
            case '#':
            case '$':
            case '&':
                break;
            case ' ':
            case '\t':
            case '\n':
                if (CheckSemantic(XamlSemantic.Element))
                {
                    RemoveSemantic(XamlSemantic.Element);
                    PushCurrentStyle(index);
                    StartAtNext(index);
                    SetCurrentStyle(_default);
                }
                if (CheckSemantic(XamlSemantic.Markup))
                {
                    PushCurrentStyle(index);
                    StartAtNext(index);
                    RemoveSemantic(XamlSemantic.Markup);
                    SetCurrentStyle(_string);
                }
                break;
            case '\r':
                break;
            default:
                break;
        }
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddSemanticWhenNo(XamlSemantic newSemantic, params XamlSemantic[] negativeSemantics)
    {
        if (negativeSemantics.All(semantic => _context.Semantics.HasFlag(semantic) == false))
        {
            _context.Semantics |= newSemantic;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void AddSemantic(XamlSemantic newSemantic) => _context.Semantics |= newSemantic;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void RemoveSemantic(XamlSemantic newSemantic) => _context.Semantics &= ~newSemantic;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] private bool CheckSemantic(XamlSemantic newSemantic) => _context.Semantics.HasFlag(newSemantic);



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckChar(int index, int offset, char c)
    {
        return _context.Code.Length > index + offset && _context.Code[index + offset] == c;
    }

    private bool CheckCharBeforeChar(int index, char c, params char[] chs)
    {
        while (true)
        {
            if (index >= _context.Code.Length)
            {
                return false;
            }
            if (chs.Contains(_context.Code[index]))
            {
                return false;
            }
            if (_context.Code[index] == c)
            {
                return true;
            }
            index++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void StartAtNext(int index)
    {
        _startIndex = index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveStart(int i)
    {
        if (_startIndex + i > 0)
        {
            _startIndex += i;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PopupStyle()
    {
        if (_styles.Count > 0)
        {
            _styles.RemoveAt(_styles.Count - 1);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushCurrentStyle(int index)
    {
        if (ReferenceEquals(_currentRef.Object, null) == false)
        {
            _styles.Add(new ValueSpan<TextRunProperties>(_startIndex, index - _startIndex, _currentRef.Object));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushSingleCharStyle(char ch, int index)
    {
        var style = GetSingleCharStyle(ch);
        _styles.Add(new ValueSpan<TextRunProperties>(index, 1, style));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetCurrentStyle(TextRunProperties style)
    {
        _currentRef.Object = style;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TextRunProperties GetSingleCharStyle(char ch)
    {
        switch (ch)
        {
            case '<':
            case '>':
            case '/':
                return _salient;
            case '"':
                return _gentle;
            case '=':
                return _equalSign;
            case '{':
            case '}':
            case ':':
            case '.':
            case '|':
            case '#':
            case '$':
            case '&':
            default:
                return _striking;
        }
    }
}
