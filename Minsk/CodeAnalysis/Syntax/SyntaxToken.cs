using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    public class SyntaxToken : SyntaxNode
    {

        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
            Span = new TextSpan(position, text?.Length ?? 0);
        }

        public override SyntaxKind Kind { get; }

        public int Position { get; }

        public string Text { get; }

        public object Value { get; }

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
    }
}
