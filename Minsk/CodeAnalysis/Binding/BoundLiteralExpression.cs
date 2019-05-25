using System;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override Type Type => Value.GetType();
        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public object Value { get; }
    }
}