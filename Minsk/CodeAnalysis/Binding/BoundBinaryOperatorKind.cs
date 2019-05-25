namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        LogicalAnd,
        LogicalOr,
        Equals, 
        NotEquals,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        GreaterThan,
        GreaterThanOrEquals,
        LessThanOrEquals,
        LessThan,
    }
}