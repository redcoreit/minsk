namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement statement, BoundStatement elseClause)
        {
            Condition = condition;
            Statement = statement;
            ElseClause = elseClause;
        }

        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }
        public BoundStatement ElseClause { get; }
    }
}