using System;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override TypeSymbol Type => Variable.Type;
        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public VariableSymbol Variable { get; }
    }
}