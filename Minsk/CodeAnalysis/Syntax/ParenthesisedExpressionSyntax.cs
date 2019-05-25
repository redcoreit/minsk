using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class ParenthesisedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesisedExpressionSyntax(SyntaxToken openParenthesis, ExpressionSyntax expression, SyntaxToken closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesisedExpression;

        public SyntaxToken OpenParenthesis { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesis { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesis;
            yield return Expression;
            yield return CloseParenthesis;
        }
    }
}
