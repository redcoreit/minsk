using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Syntax;
using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{

    public class LexerTests
    {
        [Fact]
        public void Lexer_lexes_all_tokens()
        {
            var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
                            .Cast<SyntaxKind>()
                            .Where(m => m.ToString().EndsWith("Keyword") || m.ToString().EndsWith("Token"));

            var testedTokenKinds = Enumerable.Concat(GetTokens(), GetSeparators()).Select(m => m.Kind);

            var untestedTokenKinds = new HashSet<SyntaxKind>(tokenKinds);
            untestedTokenKinds.Remove(SyntaxKind.BadToken);
            untestedTokenKinds.Remove(SyntaxKind.EndOfFileToken);
            untestedTokenKinds.ExceptWith(testedTokenKinds);

            Assert.Empty(untestedTokenKinds);
        }

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_lexes_token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_lexes_token_pairs(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + t2Text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);
            Assert.Equal(t1Kind, tokens[0].Kind);
            Assert.Equal(t1Text, tokens[0].Text);
            Assert.Equal(t2Kind, tokens[1].Kind);
            Assert.Equal(t2Text, tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeparatorData))]
        public void Lexer_lexes_token_pairs_with_separator(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + separatorText + t2Text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);
            Assert.Equal(t1Kind, tokens[0].Kind);
            Assert.Equal(t1Text, tokens[0].Text);
            Assert.Equal(separatorKind, tokens[1].Kind);
            Assert.Equal(separatorText, tokens[1].Text);
            Assert.Equal(t2Kind, tokens[2].Kind);
            Assert.Equal(t2Text, tokens[2].Text);
        }

        [Fact]
        public void Lexer_lexes_unknown_char()
        {
            var tokens = SyntaxTree.ParseTokens(".").ToArray();
            var token = Assert.Single(tokens);
            Assert.Equal(SyntaxKind.BadToken, token.Kind);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in Enumerable.Concat(GetTokens(), GetSeparators()))
            {
                yield return new object[] { t.Kind, t.Text };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
            {
                yield return new object[] { t.t1Kind, t.t1Text, t.t2Kind, t.t2Text };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
            {
                yield return new object[] { t.t1Kind, t.t1Text, t.separatorKind, t.spearatorText, t.t2Kind, t.t2Text };
            }
        }

        private static IEnumerable<(SyntaxKind Kind, string Text)> GetTokens()
        {
            var staticTokens = Enum.GetValues(typeof(SyntaxKind))
                                    .Cast<SyntaxKind>()
                                    .Select(m => (Kind: m, Text: SyntaxFacts.GetText(m)))
                                    .Where(m => m.Text != null)
                                    ;


            var dynamicTokens = new[]
            {
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
                (SyntaxKind.NumberToken,"1"),
                (SyntaxKind.NumberToken,"123"),
            };

            return Enumerable.Concat(staticTokens, dynamicTokens);
        }

        private static IEnumerable<(SyntaxKind Kind, string Text)> GetSeparators() => new[]
            {
                (SyntaxKind.WhiteSpaceToken," "),
                (SyntaxKind.WhiteSpaceToken,"  "),
                (SyntaxKind.WhiteSpaceToken,"\r"),
                (SyntaxKind.WhiteSpaceToken,"\n"),
                (SyntaxKind.WhiteSpaceToken,"\r\n"),
                (SyntaxKind.WhiteSpaceToken,"\t"),
            };


        private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.Kind, t2.Kind))
                        yield return (t1.Kind, t1.Text, t2.Kind, t2.Text);
                }
            }
        }

        private static IEnumerable<(
            SyntaxKind t1Kind,
            string t1Text,
            SyntaxKind separatorKind,
            string spearatorText,
            SyntaxKind t2Kind,
            string t2Text)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.Kind, t2.Kind))
                    {
                        foreach (var s in GetSeparators())
                            yield return (t1.Kind, t1.Text, s.Kind, s.Text, t2.Kind, t2.Text);
                    }
                }
            }
        }

        private static bool RequiresSeparator(SyntaxKind t1Kind, SyntaxKind t2Kind)
        {
            var t1IsKeyword = t1Kind.ToString().EndsWith("Keyword");
            var t2IsKeyword = t2Kind.ToString().EndsWith("Keyword");

            if (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken)
                return true;

            if (t1IsKeyword && t2IsKeyword)
                return true;

            if (t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken)
                return true;

            if (t2IsKeyword && t1Kind == SyntaxKind.IdentifierToken)
                return true;

            if (t1Kind == SyntaxKind.NumberToken && t2Kind == SyntaxKind.NumberToken)
                return true;

            if (t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            if (t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsEqualsToken)
                return true;

            if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsEqualsToken)
                return true;

            if (t1Kind == SyntaxKind.AndToken && t2Kind == SyntaxKind.AndToken)
                return true;

            if (t1Kind == SyntaxKind.AndToken && t2Kind == SyntaxKind.AndAndToken)
                return true;

            if (t1Kind == SyntaxKind.PipeToken && t2Kind == SyntaxKind.PipeToken)
                return true;

            if (t1Kind == SyntaxKind.PipeToken && t2Kind == SyntaxKind.PipePipeToken)
                return true;

            if (t1Kind == SyntaxKind.LessToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            if (t1Kind == SyntaxKind.LessToken && t2Kind == SyntaxKind.EqualsEqualsToken)
                return true;

            if (t1Kind == SyntaxKind.GreaterToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            if (t1Kind == SyntaxKind.GreaterToken && t2Kind == SyntaxKind.EqualsEqualsToken)
                return true;


            // TODO: (implement) more cases

            return false;
        }
    }
}
