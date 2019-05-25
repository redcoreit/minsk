using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFact_GetText_round_trips(SyntaxKind kind)
        {
            var text = SyntaxFacts.GetText(kind);
            if(string.IsNullOrEmpty(text))
                return;

            var tokens = SyntaxTree.ParseTokens(text);
            
            var token = Assert.Single(tokens);
            Assert.Equal(text, token.Text);
            Assert.Equal(kind, token.Kind);
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var values = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in values)
            {
                yield return new object[] { kind };
            }
        }
    }
}
