using System;
using System.Collections;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private List<Diagnostic> _diagnostics;

        public DiagnosticBag()
        {
            _diagnostics = new List<Diagnostic>();
        }

        public DiagnosticBag(DiagnosticBag bag)
        {
            _diagnostics = new List<Diagnostic>(bag);
        }

        public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type)
        {
            Report(span, $"The number {text} isn't a valid {type.Name}.");
        }

        public void ReportBadCharacter(int position, char character)
        {
            Report(new TextSpan(position, 1), $"Bad character '{character}'.");
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind currentKind, SyntaxKind expectedKind)
        {
            Report(span,  $"Unexpected token <{currentKind}>, expected token <{expectedKind}>.");
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
        {
            Report(span, $"Unary operator '{operatorText}' not defined for type '{operandType.Name}'.");
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            Report(span, $"Binary operator '{operatorText}' not defined for types '{leftOperandType.Name}' and '{rightOperandType.Name}'.");
        }

        public void ReportUndefinedName(TextSpan span, object name)
        {
            Report(span, $"Variable '{name}' doesn't exist.");
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            Report(span, $"Variable '{name}' is already declared.");
        }

        public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            Report(span, $"Type conversion failed from '{fromType.Name}' to '{toType.Name}'.");
        }

        public void ReportCannotAssignVariable(TextSpan span, string name)
        {
            Report(span, $"Variable '{name}' is read-only and cannot be assigned to.");
        }

        public void ReportUnterminatedString(TextSpan span)
        {
            Report(span, $"Unterminated string literal.");
        }

        public void AddRange(IEnumerable<Diagnostic> values) => _diagnostics.AddRange(values);

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _diagnostics.GetEnumerator();

        private void Report(TextSpan textSpan, string message)
        {
            _diagnostics.Add(new Diagnostic(textSpan, message));
        }
    }
}
