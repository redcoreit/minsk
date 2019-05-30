using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelTag label, BoundExpression condition, bool jumpIf = false)
        {
            Label = label;
            Condition = condition;
            JumpIf = jumpIf;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;
        public LabelTag Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIf { get; }
    }
}