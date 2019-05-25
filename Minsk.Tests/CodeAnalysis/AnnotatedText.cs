using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Minsk.CodeAnalysis.Text;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            var builder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            var position = 0;
            foreach (var c in text)
            {
                switch (c)
                {
                    case '[':
                        {
                            startStack.Push(position);
                            break;
                        }
                    case ']':
                        {
                            if (startStack.Count == 0)
                            {
                                throw new ArgumentException($"Too many ']' in text.", nameof(text));
                            }

                            var start = startStack.Pop();
                            spanBuilder.Add(TextSpan.FromBounds(start, position));
                            break;
                        }
                    default:
                        {
                            builder.Append(c);
                            position++;
                            break;
                        }
                }
            }

            if (startStack.Count != 0)
            {
                throw new ArgumentException($"Missing ']' in text.", nameof(text));
            }

            return new AnnotatedText(builder.ToString(), spanBuilder.ToImmutableArray());
        }

        private static string Unindent(string text)
        {
            var lines = UnindentLines(text);
            return string.Join(Environment.NewLine, lines);
        }

        public static IReadOnlyList<string> UnindentLines(string text)
        {
            var lines = new List<string>();

            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            var minIndentation = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                var indentation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    continue;
                }

                lines[i] = lines[i].Substring(minIndentation);
            }

            while (lines.Count > 0 && string.IsNullOrEmpty(lines[0]))
            {
                lines.RemoveAt(0);
            }

            while (lines.Count > 0 && string.IsNullOrEmpty(lines[lines.Count - 1]))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines;
        }
    }
}
