using System;
using System.Collections.Generic;
using System.Text;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private DiagnosticBag _diagnostics;
        private readonly SourceText _text;

        private int _position;

        private int _start;
        private object _value;
        private SyntaxKind _kind;

        public Lexer(SourceText text)
        {
            _text = text;
            _diagnostics = new DiagnosticBag();
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        internal SyntaxToken Lex()
        {
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    {
                        _kind = SyntaxKind.EndOfFileToken;
                        break;
                    }
                case '+':
                    {
                        MoveNext();
                        _kind = SyntaxKind.PlusToken;
                        break;
                    }
                case '-':
                    {
                        MoveNext();
                        _kind = SyntaxKind.MinusToken;
                        break;
                    }
                case '*':
                    {
                        MoveNext();
                        _kind = SyntaxKind.StarToken;
                        break;
                    }
                case '/':
                    {
                        MoveNext();
                        _kind = SyntaxKind.ForwardSlashToken;
                        break;
                    }
                case '(':
                    {
                        MoveNext();
                        _kind = SyntaxKind.OpenParenthesisToken;
                        break;
                    }
                case ')':
                    {
                        MoveNext();
                        _kind = SyntaxKind.CloseParenthesisToken;
                        break;
                    }
                case '{':
                    {
                        MoveNext();
                        _kind = SyntaxKind.OpenBraceToken;
                        break;
                    }
                case '}':
                    {
                        MoveNext();
                        _kind = SyntaxKind.CloseBraceToken;
                        break;
                    }
                case '~':
                    {
                        MoveNext();
                        _kind = SyntaxKind.TildeToken;
                        break;
                    }
                case '^':
                    {
                        MoveNext();
                        _kind = SyntaxKind.HatToken;
                        break;
                    }
                case '!':
                    {
                        if (LookAhead == '=')
                        {
                            MoveNext(2);
                            _kind = SyntaxKind.BangEqualsToken;
                        }
                        else
                        {
                            MoveNext();
                            _kind = SyntaxKind.BangToken;
                        }
                        break;
                    }
                case '&':
                    {
                        if (LookAhead == '&')
                        {
                            MoveNext(2);
                            _kind = SyntaxKind.AndAndToken;
                        }
                        else
                        {
                            MoveNext();
                            _kind = SyntaxKind.AndToken;
                        }
                        break;
                    }
                case '|':
                    {
                        if (LookAhead == '|')
                        {
                            MoveNext(2);
                            _kind = SyntaxKind.PipePipeToken;
                        }
                        else
                        {
                            MoveNext();
                            _kind = SyntaxKind.PipeToken;
                        }
                        break;
                    }
                case '=':
                    {
                        if (LookAhead == '=')
                        {
                            MoveNext(2);
                            _kind = SyntaxKind.EqualsEqualsToken;
                        }
                        else
                        {
                            MoveNext();
                            _kind = SyntaxKind.EqualsToken;
                        }
                        break;
                    }
                case '<':
                    {
                        if (LookAhead == '=')
                        {
                            MoveNext(2);
                            _kind = SyntaxKind.LessOrEqualsToken;
                        }
                        else
                        {
                            MoveNext();
                            _kind = SyntaxKind.LessToken;
                        }
                        break;
                    }
                case '>':
                    {
                        if (LookAhead == '=')
                        {
                            MoveNext(2);
                            _kind = SyntaxKind.GreaterOrEqualsToken;
                        }
                        else
                        {
                            MoveNext();
                            _kind = SyntaxKind.GreaterToken;
                        }
                        break;
                    }
                case '"':
                    {
                        ReadString();
                        break;
                    }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        ReadNumbersToken();
                        break;
                    }
                case ' ':
                case '\t':
                    {
                        ReadWhitespaceTokens();
                        break;
                    }
                default:
                    {
                        if (char.IsLetter(Current))
                        {
                            ReadLettersToken();
                        }
                        else if (char.IsWhiteSpace(Current))
                        {
                            ReadWhitespaceTokens();
                        }
                        else
                        {
                            _diagnostics.ReportBadCharacter(_position, Current);
                            MoveNext();
                        }
                        break;
                    }
            }

            {
                var text = SyntaxFacts.GetText(_kind);
                var length = _position - _start;
                if (text is null)
                {
                    text = _text.ToString(_start, length);
                }

                return new SyntaxToken(_kind, _start, text, _value);
            }

            void ReadNumbersToken()
            {
                do
                    MoveNext();
                while (char.IsDigit(Current));

                var length = _position - _start;
                var text = _text.ToString(_start, length);

                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, TypeSymbol.Int);
                }

                _kind = SyntaxKind.NumberToken;
                _value = value;
            }

            void ReadLettersToken()
            {
                do
                    MoveNext();
                while (char.IsLetter(Current));

                var text = _text.ToString(_start, _position - _start);
                _kind = SyntaxFacts.GetKeywordKind(text);
            }

            void ReadWhitespaceTokens()
            {
                do
                    MoveNext();
                while (char.IsWhiteSpace(Current));

                _kind = SyntaxKind.WhiteSpaceToken;
            }

            void ReadString()
            {
                MoveNext();

                var done = false;
                var builder = new StringBuilder();

                while (!done)
                {
                    switch (Current)
                    {
                        case '\0':
                        case '\r':
                        case '\n':
                            var span = new TextSpan(_start, 1);
                            Diagnostics.ReportUnterminatedString(span);
                            done = true;
                            break;
                        case '\\':
                            {
                                if (LookAhead == '"')
                                {
                                    MoveNext();
                                }
                                builder.Append(Current);
                                MoveNext();
                                break;
                            }
                        case '"':
                            {
                                done = true;
                                MoveNext();
                                break;
                            }
                        default:
                            {
                                builder.Append(Current);
                                MoveNext();
                                break;
                            }
                    }
                }

                _kind = SyntaxKind.StringToken;
                _value = builder.ToString();
            }
        }

        private void MoveNext(int offset = 1) => _position += offset;

        private char Current => Peek(0);

        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index < _text.Length)
            {
                return _text[index];
            }

            return '\0';
        }
    }
}
