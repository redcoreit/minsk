using System;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public BoundExpression()
        {
        }

        public abstract TypeSymbol Type { get; }
    }
}