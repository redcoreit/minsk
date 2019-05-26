using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    internal sealed class Parser
    {
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private readonly SourceText _text;
        private int _position;
        private DiagnosticBag _diagnostics;

        public Parser(SourceText text)
        {
            _text = text;
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            _diagnostics = new DiagnosticBag(lexer.Diagnostics);

            SyntaxToken token;

            do
            {
                token = lexer.Lex();

                if (token.Kind == SyntaxKind.BadToken)
                {
                    continue;
                }

                if (token.Kind == SyntaxKind.WhiteSpaceToken)
                {
                    continue;
                }

                tokens.Add(token);

            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToImmutableArray();
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = ExpectToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclarationStatement();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxKind.ForKeyword:
                    return ParseForStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private ForStatementSyntax ParseForStatement()
        {
            var forkeyword = ExpectToken(SyntaxKind.ForKeyword);
            var identifier = ExpectToken(SyntaxKind.IdentifierToken);
            var equalsToken = ExpectToken(SyntaxKind.EqualsToken);
            var lowerBound = ParseExpression();
            var toKeyword = ExpectToken(SyntaxKind.ToKeyword);
            var upperBound = ParseExpression();
            var statement = ParseStatement();

            return new ForStatementSyntax(forkeyword, identifier, equalsToken, lowerBound, toKeyword, upperBound, statement);
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            var keyword = ExpectToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();

            return new WhileStatementSyntax(keyword, condition, statement);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            var keyword = ExpectToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseClause = ParseElseClause();

            return new IfStatementSyntax(keyword, condition, statement, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }

            var keyword = ExpectToken(SyntaxKind.ElseKeyword);
            var statement = ParseStatement();

            return new ElseClauseSyntax(keyword, statement);
        }

        private StatementSyntax ParseVariableDeclarationStatement()
        {
            var expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;

            var keyword = ExpectToken(expected);
            var identifier = ExpectToken(SyntaxKind.IdentifierToken);
            var equals = ExpectToken(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();

            return new VariableDeclarationSyntax(keyword, identifier, equals, initializer);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openBrace = ExpectToken(SyntaxKind.OpenBraceToken);

            while (Current.Kind != SyntaxKind.EndOfFileToken && Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var startToken = Current;
                var statement = ParseStatement();
                statements.Add(statement);

                // if ParseStatement() did not consume any tokens,
                // let's skip the current token and continue.
                // We do not need to report error because we'll
                // already tried to parse expression statement.
                if (Current == startToken)
                {
                    ReadAndMoveNext();
                }
            }

            var closeBrace = ExpectToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBrace, statements.ToImmutable(), closeBrace);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Current.Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = ReadAndMoveNext();
                var operatorToken = ReadAndMoveNext();
                var right = ParseAssignmentExpression();

                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecendence = 0)
        {
            ExpressionSyntax left;
            var unaryPrecendence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryPrecendence != 0 && unaryPrecendence >= parentPrecendence)
            {
                var operatorToken = ReadAndMoveNext();
                var operand = ParseBinaryExpression(unaryPrecendence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var binaryPrecendence = Current.Kind.GetBinaryOperatorPrecedence();
                if (binaryPrecendence == 0 || binaryPrecendence <= parentPrecendence)
                {
                    break;
                }

                var operatorToken = ReadAndMoveNext();
                var right = ParseBinaryExpression(binaryPrecendence);

                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return ParseParenthesisedExpression();
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteral();
                case SyntaxKind.NumberToken:
                    return ParseNumberExpression();
                case SyntaxKind.StringToken:
                    return ParseStringExpression();
                case SyntaxKind.IdentifierToken:
                default:
                    return PareseNameExpression();
            }
        }

        private ExpressionSyntax PareseNameExpression()
        {
            var identifier = ExpectToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifier);
        }

        private ExpressionSyntax ParseParenthesisedExpression()
        {
            var openParenthesis = ReadAndMoveNext();
            var expression = ParseExpression();
            var closeParenthesis = ExpectToken(SyntaxKind.CloseParenthesisToken);

            return new ParenthesisedExpressionSyntax(openParenthesis, expression, closeParenthesis);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            var keywordToken = ReadAndMoveNext();
            var value = keywordToken.Kind == SyntaxKind.TrueKeyword;
            return new LiteralExpressionSyntax(keywordToken, value);
        }

        private ExpressionSyntax ParseNumberExpression()
        {
            var numberToken = ExpectToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseStringExpression()
        {
            var stringToken = ExpectToken(SyntaxKind.StringToken);
            return new LiteralExpressionSyntax(stringToken);
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _tokens.Count())
            {
                return _tokens.LastOrDefault();
            }

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken ReadAndMoveNext()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken ExpectToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
            {
                return ReadAndMoveNext();
            }

            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }

    }
}
