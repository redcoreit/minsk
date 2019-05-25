using System;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
            : this(syntaxKind, kind, type, type, type)
        {
        }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type, Type resultType)
            : this(syntaxKind, kind, type, type, resultType)
        {
        }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            Type = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type Type { get; }

        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.AndAndToken,BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipePipeToken,BoundBinaryOperatorKind.LogicalOr, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PlusToken,BoundBinaryOperatorKind.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusToken,BoundBinaryOperatorKind.Subtraction, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarToken,BoundBinaryOperatorKind.Multiplication, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.ForwardSlashToken,BoundBinaryOperatorKind.Division, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken,BoundBinaryOperatorKind.Equals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken,BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken,BoundBinaryOperatorKind.Equals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken,BoundBinaryOperatorKind.NotEquals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.GreaterToken,BoundBinaryOperatorKind.GreaterThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken,BoundBinaryOperatorKind.GreaterThanOrEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken,BoundBinaryOperatorKind.LessThanOrEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.LessToken,BoundBinaryOperatorKind.LessThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.AndToken,BoundBinaryOperatorKind.BitwiseAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipeToken,BoundBinaryOperatorKind.BitwiseOr, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.HatToken,BoundBinaryOperatorKind.BitwiseXor, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.AndToken,BoundBinaryOperatorKind.BitwiseAnd, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.PipeToken,BoundBinaryOperatorKind.BitwiseOr, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.HatToken,BoundBinaryOperatorKind.BitwiseXor, typeof(int)),
        };

        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
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