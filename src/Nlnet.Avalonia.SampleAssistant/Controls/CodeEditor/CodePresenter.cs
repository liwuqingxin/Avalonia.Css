using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Nlnet.Avalonia.SampleAssistant
{
    public class CodePresenter : TextPresenter
    {
        private string? _lastCode;
        private TextLayout? _lastLayout;

        protected override TextLayout CreateTextLayout()
        {
            var text = GetText();

            // Cache it to improve performance.
            if (text == _lastCode && _lastLayout != null)
            {
                return _lastLayout;
            }

            var typeface = new Typeface(FontFamily, FontStyle, FontWeight);
            var selectionStart = CoerceCaretIndex(SelectionStart);
            var selectionEnd = CoerceCaretIndex(SelectionEnd);
            var start = Math.Min(selectionStart, selectionEnd);
            var length = Math.Max(selectionStart, selectionEnd) - start;
            var foreground = Foreground;

            _lastCode = text;

            if (string.IsNullOrEmpty(text) == false)
            {
                var context = new CodeStyleContext<XamlSemantic>(text, foreground ?? Brushes.Black, typeface, FontSize, XamlSemantic.None);
                var highlighter = new XamlCodeHighlighter(context);
                _ = highlighter.TryHighlight(
                    text,
                    context,
                    out var textStyleOverrides);
                return _lastLayout = CreateTextLayoutInternal(_constraint, text, typeface, textStyleOverrides);
            }
            else
            {
                return _lastLayout = CreateTextLayoutInternal(_constraint, text, typeface, null);
            }
        }

        private string? GetText()
        {
            if (!string.IsNullOrEmpty(PreeditText))
            {
                if (string.IsNullOrEmpty(Text) || CaretIndex > Text.Length)
                {
                    return PreeditText;
                }

                var text = Text[..CaretIndex] + PreeditText + Text[CaretIndex..];

                return text;
            }

            return Text;
        }

        private int CoerceCaretIndex(int value)
        {
            var text = Text;
            var length = text?.Length ?? 0;
            return Math.Max(0, Math.Min(length, value));
        }

        /// <summary>
        /// Creates the <see cref="TextLayout"/> used to render the text.
        /// </summary>
        /// <param name="constraint">The constraint of the text.</param>
        /// <param name="text">The text to format.</param>
        /// <param name="typeface"></param>
        /// <param name="textStyleOverrides"></param>
        /// <returns>A <see cref="TextLayout"/> object.</returns>
        private TextLayout CreateTextLayoutInternal(Size constraint, string? text, Typeface typeface,
            IReadOnlyList<ValueSpan<TextRunProperties>>? textStyleOverrides)
        {
            var foreground = Foreground;
            var maxWidth = MathUtilities.IsZero(constraint.Width) ? double.PositiveInfinity : constraint.Width;
            var maxHeight = MathUtilities.IsZero(constraint.Height) ? double.PositiveInfinity : constraint.Height;

            var textLayout = new TextLayout(text, typeface, FontSize, foreground, TextAlignment,
                TextWrapping, maxWidth: maxWidth, maxHeight: maxHeight, textStyleOverrides: textStyleOverrides,
                flowDirection: FlowDirection, lineHeight: LineHeight, letterSpacing: LetterSpacing);

            return textLayout;
        }



        #region Measure And Arrange

        private Size _constraint;

        protected override Size MeasureOverride(Size availableSize)
        {
            _constraint = availableSize;

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var textWidth = Math.Ceiling(TextLayout.Bounds.Width);
            if (finalSize.Width < textWidth)
            {
                finalSize = finalSize.WithWidth(textWidth);
            }

            _constraint = new Size(Math.Ceiling(finalSize.Width), double.PositiveInfinity);

            return base.ArrangeOverride(finalSize);
        }

        #endregion
    }
}
