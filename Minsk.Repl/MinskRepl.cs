using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk
{
    internal sealed class MinskRepl : Repl
    {
        private bool _showTree;
        private bool _showBoundTree;
        private Compilation _previous;
        private Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            var syntaxTree = SyntaxTree.Parse(text);

            if (syntaxTree.Diagnostics.Any())
            {
                return false;
            }

            return true;
        }

        protected override void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                case "#st":
                    {
                        _showTree = !_showTree;
                        Console.WriteLine(_showTree ? "Showing parse tree." : "Not showing parse tree.");
                    }
                    break;
                case "#bt":
                    {
                        _showBoundTree = !_showBoundTree;
                        Console.WriteLine(_showBoundTree ? "Showing bound tree." : "Not showing bound tree.");
                    }
                    break;
                case "#cls":
                    {
                        Console.Clear();
                        break;
                    }
                case "#rst":
                    {
                        _previous = null;
                        break;
                    }
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }
        protected override void EvaluateSubmission(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = _previous == null
                                ? new Compilation(syntaxTree)
                                : _previous.ContinueWith(syntaxTree);

            var result = compilation.Evaluate(_variables);

            var diagnostics = result.Diagnostics;

            if (_showTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                syntaxTree.Root.WriteTo(Console.Out);
                Console.ResetColor();
            }

            if (_showBoundTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                compilation.WriteTreeTo(Console.Out);
                Console.ResetColor();
            }

            if (!diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(result.Value);
                Console.ResetColor();

                _previous = compilation;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (var item in diagnostics)
                {
                    var lineIndex = syntaxTree.Text.GetLineIndex(item.Span.Start);
                    var lineNumber = lineIndex + 1;
                    var character = item.Span.Start - syntaxTree.Text.Lines[lineIndex].Span.Start + 1;

                    Console.Write($"({lineNumber}, {character}): ");
                    Console.WriteLine(item);
                }
                Console.ResetColor();
            }
        }

        protected override void Render(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);
            foreach (var token in tokens)
            {
                var isKeyword = token.Kind.ToString().EndsWith("Keyword");
                var isNumber = SyntaxKind.NumberToken == token.Kind;

                if (isKeyword)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else if (!isNumber)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.Write(token.Text);
                Console.ResetColor();
            }
        }
    }
}
