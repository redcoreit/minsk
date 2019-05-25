using System;
using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Text
{
    public class SourceText
    {
        private readonly string _text;

        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }

        public ImmutableArray<TextLine> Lines { get; }

        public int Length => _text.Length;

        public char this[int index] => _text[index];

        public int GetLineIndex(int position) => BinarySearchOverLines(position);

        private int BinarySearchOverLines(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;

            while (lower <= upper)
            {
                var index = lower + (upper - lower) / 2;
                var direction = GetDirectionRelativeToLine(Lines[index]);

                switch (direction)
                {
                    case 0:
                        {
                            return index;
                        }
                    case -1:
                        {
                            upper = index - 1;
                            break;
                        }
                    case 1:
                        {
                            lower = index + 1;
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, typeof(int).Name);
                }
            }

            throw new Exception($"Position not found. Position: {position}");

            int GetDirectionRelativeToLine(TextLine line)
            {
                if (position >= line.Span.Start && position <= line.Span.End)
                {
                    return 0;
                }

                if (position < line.Span.Start)
                {
                    return -1;
                }

                if (position > line.Span.End)
                {
                    return 1;
                }

                throw new Exception($"Getting binary search direction failed!");
            }
        }

        public override string ToString() => _text;

        public string ToString(TextSpan span) => ToString(span.Start, span.Length);

        public string ToString(int start, int length) => _text.Substring(start, length);

        public static SourceText From(string text) => new SourceText(text);

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var lineStart = 0;
            var position = 0;

            while (position < text.Length)
            {
                var lineBreakLength = GetLineBreakWidth(text, position);

                if (lineBreakLength == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, lineStart, position, lineBreakLength);

                    position += lineBreakLength;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
            {
                AddLine(result, sourceText, lineStart, position, 0);
            }

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder builder, SourceText sourceText, int lineStart, int position, int lineBreakLength)
        {
            var lineLength = position - lineStart;
            var lineLengthIncludingLineBreak = lineLength + lineBreakLength;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);

            builder.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            var current = text[position];
            var lookahead = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (current == '\r' && lookahead == '\n')
            {
                return 2;
            }
            if (current == '\n' || current == '\r')
            {
                return 1;
            }

            return 0;
        }
    }
}
