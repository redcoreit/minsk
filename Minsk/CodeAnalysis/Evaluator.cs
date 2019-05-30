using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax.Binding;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Binding;

namespace Minsk.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            var labelInstructionIndexLookup = BuildLableInstructionIndexLookup();

            var index = 0;
            while (index < _root.Statements.Length)
            {
                var s = _root.Statements[index];
                switch (s)
                {
                    case BoundExpressionStatement statement:
                        {
                            Evaluate(statement);
                            index++;
                            break;
                        }
                    case BoundVariableDeclaration statement:
                        {
                            Evaluate(statement);
                            index++;
                            break;
                        }
                    case BoundConditionalGotoStatement statement:
                        {
                            var conditionValue = (bool)EvaluateExpression(statement.Condition);
                            if (statement.JumpIf == conditionValue)
                            {
                                index = labelInstructionIndexLookup[statement.Label];
                            }
                            else
                            {
                                index++;
                            }
                            break;
                        }
                    case BoundGotoStatement statement:
                        {
                            index = labelInstructionIndexLookup[statement.Label];
                            break;
                        }
                    case BoundLabelStatement statement:
                        {
                            index++;
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(s), s, null);
                }
            }

            return _lastValue;
        }

        private void Evaluate(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _variables[node.Variable] = value;
            _lastValue = value;
        }

        private void Evaluate(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch (node)
            {
                case BoundLiteralExpression expression:
                    return Evaluate(expression);
                case BoundUnaryExpression expression:
                    return Evaluate(expression);
                case BoundBinaryExpression expression:
                    return Evaluate(expression);
                case BoundVariableExpression expression:
                    return Evaluate(expression);
                case BoundAssignmentExpression expression:
                    return Evaluate(expression);
                default:
                    throw new ArgumentOutOfRangeException(nameof(node), node, null);
            }
        }

        private object Evaluate(BoundLiteralExpression literalExpression) => literalExpression.Value;

        private object Evaluate(BoundBinaryExpression binaryExpression)
        {
            var left = EvaluateExpression(binaryExpression.Left);
            var right = EvaluateExpression(binaryExpression.Right);

            switch (binaryExpression.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.LessThan:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.LessThanOrEquals:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorKind.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.GreaterThanOrEquals:
                    return (int)left >= (int)right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorKind.BitwiseAnd:
                    return BitwiseAnd(left, right);
                case BoundBinaryOperatorKind.BitwiseOr:
                    return BitwiseOr(left, right);
                case BoundBinaryOperatorKind.BitwiseXor:
                    return BitwiseXor(left, right);

                default:
                    throw new ArgumentOutOfRangeException(nameof(binaryExpression.Op.Kind), binaryExpression.Op.Kind, null);
            }
        }

        private object Evaluate(BoundUnaryExpression unaryExpression)
        {
            var operand = EvaluateExpression(unaryExpression.Operand);

            switch (unaryExpression.Op.Kind)
            {
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                case BoundUnaryOperatorKind.BitwiseNegation:
                    return ~(int)operand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unaryExpression.Op), unaryExpression.Op, null);
            }
        }

        private object Evaluate(BoundVariableExpression variableExpression) => _variables[variableExpression.Variable];

        private object Evaluate(BoundAssignmentExpression assignmentExpression)
        {
            var value = EvaluateExpression(assignmentExpression.Expression);
            _variables[assignmentExpression.Variable] = value;
            return value;
        }

        private object BitwiseAnd(object a, object b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            switch (a)
            {
                case int x when b is int y:
                    return x & y;
                case bool x when b is bool y:
                    return x & y;
                default:
                    throw new NotSupportedException($"Bitwise AND operator not supported between type '{a.GetType().Name}' and '{b.GetType().Name}'.");
            }
        }

        private object BitwiseOr(object a, object b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            switch (a)
            {
                case int x when b is int y:
                    return x | y;
                case bool x when b is bool y:
                    return x | y;
                default:
                    throw new NotSupportedException($"Bitwise OR operator not supported between type '{a.GetType().Name}' and '{b.GetType().Name}'.");
            }
        }

        private object BitwiseXor(object a, object b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            switch (a)
            {
                case int x when b is int y:
                    return x ^ y;
                case bool x when b is bool y:
                    return x ^ y;
                default:
                    throw new NotSupportedException($"Bitwise XOR operator not supported between type '{a.GetType().Name}' and '{b.GetType().Name}'.");
            }
        }

        private IReadOnlyDictionary<LabelTag, int> BuildLableInstructionIndexLookup()
        {
            var lookup = new Dictionary<LabelTag, int>();

            var index = 0;
            foreach (var statement in _root.Statements)
            {
                if (statement is BoundLabelStatement label)
                {
                    lookup.Add(label.Label, index + 1);
                }
                index++;
            }

            return lookup;
        }
    }
}
