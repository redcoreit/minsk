using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
        public LabelSymbol Label { get; }
    }
}