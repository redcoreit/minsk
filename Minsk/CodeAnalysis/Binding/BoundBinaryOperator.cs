using System;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type)
            : this(syntaxKind, kind, type, type, type)
        {
        }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type, TypeSymbol resultType)
            : this(syntaxKind, kind, type, type, resultType)
        {
        }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            Type = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol Type { get; }

        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.AndAndToken,BoundBinaryOperatorKind.LogicalAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.PipePipeToken,BoundBinaryOperatorKind.LogicalOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.PlusToken,BoundBinaryOperatorKind.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.MinusToken,BoundBinaryOperatorKind.Subtraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.StarToken,BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.ForwardSlashToken,BoundBinaryOperatorKind.Division, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken,BoundBinaryOperatorKind.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken,BoundBinaryOperatorKind.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken,BoundBinaryOperatorKind.Equals, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken,BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterToken,BoundBinaryOperatorKind.GreaterThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken,BoundBinaryOperatorKind.GreaterThanOrEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken,BoundBinaryOperatorKind.LessThanOrEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.LessToken,BoundBinaryOperatorKind.LessThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.AndToken,BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.PipeToken,BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.HatToken,BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.AndToken,BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.PipeToken,BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.HatToken,BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Int),
        };

        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol leftType, TypeSymbol rightType)
        {
            foreach (var op in _operators)
            {
                if (op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
                {
                    return op;
                }
            }

            return null;
        }
    }
}