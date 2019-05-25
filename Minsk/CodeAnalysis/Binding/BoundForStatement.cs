using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(VariableSymbol variable, BoundExpression lowerBound, BoundExpression upperBound, BoundStatement statement)
        {
            Variable = variable;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Statement = statement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
        public VariableSymbol Variable { get; }
        public BoundExpression LowerBound { get; }
        public BoundExpression UpperBound { get; }
        public BoundStatement Statement { get; }
    }
}