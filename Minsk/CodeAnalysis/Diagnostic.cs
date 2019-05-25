using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan textSpan, string message)
        {
            Span = textSpan;
            Message = message;
        }

        public TextSpan Span { get; }
        public string Message { get; }

        public override string ToString() => Message;
    }
}
