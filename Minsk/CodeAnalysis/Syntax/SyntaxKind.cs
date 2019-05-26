namespace Minsk.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        EndOfFileToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        ForwardSlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        BadToken,
        WhiteSpaceToken,
        IdentifierToken,
        BangToken,
        AndAndToken,
        PipePipeToken,
        EqualsEqualsToken,
        EqualsToken,
        BangEqualsToken,
        TildeToken,
        AndToken,
        PipeToken,
        HatToken,
        LessOrEqualsToken,
        LessToken,
        GreaterToken,
        GreaterOrEqualsToken,
        StringToken,


        // Keywords
        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,

        // Expressions
        LiteralExpression,
        NameExpression,
        BinaryExpression,
        ParenthesisedExpression,
        UnaryExpression,
        AssignmentExpression,

        // Others
        CompilationUnit,
        ElseClause,
        
        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,
    }
}
