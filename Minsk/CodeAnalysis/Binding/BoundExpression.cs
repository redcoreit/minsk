using System;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public BoundExpression()
        {
        }

        public abstract Type Type { get; }
    }
}