using System;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.BangToken:
                case SyntaxKind.TildeToken:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.ForwardSlashToken:
                    return 5;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken:
                case SyntaxKind.LessOrEqualsToken:
                case SyntaxKind.LessToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualsToken:
                    return 3;
                case SyntaxKind.AndToken:
                case SyntaxKind.AndAndToken:
                    return 2;
                case SyntaxKind.PipeToken:
                case SyntaxKind.PipePipeToken:
                case SyntaxKind.HatToken:
                    return 1;
                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                case "let":
                    return SyntaxKind.LetKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                case "to":
                    return SyntaxKind.ToKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static string GetText(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.ForwardSlashToken:
                    return "/";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";
                case SyntaxKind.BangToken:
                    return "!";
                case SyntaxKind.AndAndToken:
                    return "&&";
                case SyntaxKind.PipePipeToken:
                    return "||";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.BangEqualsToken:
                    return "!=";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.LetKeyword:
                    return "let";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.TildeToken:
                    return "~";
                case SyntaxKind.PipeToken:
                    return "|";
                case SyntaxKind.AndToken:
                    return "&";
                case SyntaxKind.HatToken:
                    return "^";
                case SyntaxKind.GreaterOrEqualsToken:
                    return ">=";
                case SyntaxKind.GreaterToken:
                    return ">";
                case SyntaxKind.LessToken:
                    return "<";
                case SyntaxKind.LessOrEqualsToken:
                    return "<=";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.WhileKeyword:
                    return "while";
                case SyntaxKind.ForKeyword:
                    return "for";
                case SyntaxKind.ToKeyword:
                    return "to";
                default: return null;
            }
        }
    }
}