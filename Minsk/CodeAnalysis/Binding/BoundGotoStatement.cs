using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(LabelTag label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.GotoStatement;
        public LabelTag Label { get; }
    }
}