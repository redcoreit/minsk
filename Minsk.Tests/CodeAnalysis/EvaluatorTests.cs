using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Syntax;
using Xunit;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("~1", -2)]
        [InlineData("1 - 2", -1)]
        [InlineData("1 * 2", 2)]
        [InlineData("4 / 2", 2)]
        [InlineData("(10)", 10)]
        [InlineData("12 == 3", false)]
        [InlineData("3 == 3", true)]
        [InlineData("4 != 3", true)]
        [InlineData("3 != 3", false)]
        [InlineData("3 > 3", false)]
        [InlineData("3 > 2", true)]
        [InlineData("3 >= 4", false)]
        [InlineData("3 >= 3", true)]
        [InlineData("3 < 3", false)]
        [InlineData("2 < 3", true)]
        [InlineData("4 <= 3", false)]
        [InlineData("3 <= 3", true)]
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 3", 1)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("1 ^ 3", 2)]
        [InlineData("true != false", true)]
        [InlineData("true == false", false)]
        [InlineData("false == false", true)]
        [InlineData("false | false", false)]
        [InlineData("false | true", true)]
        [InlineData("true | false", true)]
        [InlineData("true | true", true)]
        [InlineData("false & false", false)]
        [InlineData("false & true", false)]
        [InlineData("true & false", false)]
        [InlineData("true & true", true)]
        [InlineData("false ^ false", false)]
        [InlineData("false ^ true", true)]
        [InlineData("true ^ false", true)]
        [InlineData("true ^ true", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ var a = 0 (a = 10) * a }", 100)]
        [InlineData("{ var a = 0 if a == 0 a = 10 a }", 10)]
        [InlineData("{ var a = 0 if a != 0 a = 10 a }", 0)]
        [InlineData("{ var a = 0 if a != 0 a = 10 else a = 2 a }", 2)]
        [InlineData("{ var i = 10 var result = 0 while i > 0 { result = result + 1 i = i - 1 } result }", 10)]
        [InlineData("{ var result = 0 for i = 0 to 10 { result = result + 1 } result }", 10)]
        [InlineData("{ var a = 10 for i = 1 to (a = a - 1) { } a }", 9)]
        public void Evaulator_computes_correct_values(string text, object expectedResult)
        {
            AssertValue(text, expectedResult);
        }

        [Fact]
        public void Evaluator_variable_declaration_reports_redeclaration()
        {
            var text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 2
                    }
                    var [x] = 1
                }
            ";

            var diagnostics = @"
                Variable 'x' is already declared.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_block_statement_no_infinite_loop()
        {
            var text = @"
                {
                [)][]
            ";

            var diagnostics = @"
                Unexpected token <CloseParenthesisToken>, expected token <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected token <CloseBraceToken>.
             ";

            AssertDiagnostics(text, diagnostics);
        }


        [Fact]
        public void Evaluator_name_reports_undefined()
        {
            var text = @"[x] + 10";

            var diagnostics = @"
                Variable 'x' doesn't exist.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_name_reports_no_error_for_inserted_token()
        {
            var text = @"[]";

            var diagnostics = @"
                Unexpected token <EndOfFileToken>, expected token <IdentifierToken>.
             ";
             
            AssertDiagnostics(text, diagnostics);
        }


        [Fact]
        public void Evaluator_assign_reports_undefined()
        {
            var text = @"[x] = 10";

            var diagnostics = @"
                Variable 'x' doesn't exist.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_assign_reports_cannot_assign()
        {
            var text = @"
                    {
                        let x = 10
                        x [=] 1
                    }
            ";

            var diagnostics = @"
                Variable 'x' is read-only and cannot be assigned to.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_assign_reports_cannot_convert()
        {
            var text = @"
                    {
                        var x = 10
                        x = [false]
                    }
            ";

            var diagnostics = @"
                Type conversion failed from 'bool' to 'int'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_if_statement_reports_cannot_convert()
        {
            var text = @"
                    {
                        var x = 10
                        if [10]
                            x = 10
                    }
            ";

            var diagnostics = @"
                Type conversion failed from 'int' to 'bool'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_while_statement_reports_cannot_convert()
        {
            var text = @"
                    {
                        var x = 10
                        while [10]
                            x = 10
                    }
            ";

            var diagnostics = @"
                Type conversion failed from 'int' to 'bool'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_for_statement_reports_cannot_convert_lower_bound()
        {
            var text = @"
                    {
                        var x = 10
                        for i = [false] to 10
                            x = 10
                    }
            ";

            var diagnostics = @"
                Type conversion failed from 'bool' to 'int'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_for_statement_reports_cannot_convert_upper_bound()
        {
            var text = @"
                    {
                        var x = 10
                        for i = 0 to [false]
                            x = 10
                    }
            ";

            var diagnostics = @"
                Type conversion failed from 'bool' to 'int'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_unary_reports_undefined()
        {
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator '+' not defined for type 'bool'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_binary_reports_undefined()
        {
            var text = @"false [+] 1";

            var diagnostics = @"
                Binary operator '+' not defined for types 'bool' and 'int'.
             ";

            AssertDiagnostics(text, diagnostics);
        }

        private static void AssertValue(string text, object expectedResult)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedResult, result.Value);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (expectedDiagnostics.Count != annotatedText.Spans.Length)
            {
                throw new Exception("ERROR: Mark all diagnostic occurence in text.");
            }

            Assert.Equal(expectedDiagnostics.Count, result.Diagnostics.Length);

            for (int i = 0; i < expectedDiagnostics.Count; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;

                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
