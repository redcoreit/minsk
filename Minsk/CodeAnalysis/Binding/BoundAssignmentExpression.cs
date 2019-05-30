using System;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override TypeSymbol Type => Expression.Type;
        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}