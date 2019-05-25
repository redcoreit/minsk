using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelSymbol label, BoundExpression condition, bool jumpIf = false)
        {
            Label = label;
            Condition = condition;
            JumpIf = jumpIf;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;
        public LabelSymbol Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIf { get; }
    }
}