using System;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override TypeSymbol Type => TypeSymbol.FromClrType(Value);
        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public object Value { get; }
    }
}