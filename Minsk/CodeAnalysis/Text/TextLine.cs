namespace Minsk.CodeAnalysis.Text
{
    public class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthIncludingLineBreak)
        {
            Text = text;
            Span = new TextSpan(start, length);
            SpanIncludingLineBreak = new TextSpan(start, lengthIncludingLineBreak);
        }

        public SourceText Text { get; }
        public TextSpan Span { get; }
        public TextSpan SpanIncludingLineBreak { get; }

        public override string ToString() => Text.ToString(Span);
    }
}
