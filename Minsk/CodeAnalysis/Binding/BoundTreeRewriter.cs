using System;
using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal abstract class BoundTreeRewriter
    {
        public BoundStatement RewriteStatement(BoundStatement node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    {
                        return RewriteBlockStatement(((BoundBlockStatement)node));
                    }
                case BoundNodeKind.ExpressionStatement:
                    {
                        return RewriteExpressionStatement(((BoundExpressionStatement)node));
                    }
                case BoundNodeKind.VariableDeclaration:
                    {
                        return RewriteVariableDeclaration(((BoundVariableDeclaration)node));
                    }
                case BoundNodeKind.IfStatement:
                    {
                        return RewriteIfStatement(((BoundIfStatement)node));
                    }
                case BoundNodeKind.WhileStatement:
                    {
                        return RewriteWhileStatement(((BoundWhileStatement)node));
                    }
                case BoundNodeKind.ForStatement:
                    {
                        return RewriteForStatement(((BoundForStatement)node));
                    }
                case BoundNodeKind.GotoStatement:
                    {
                        return RewriteGotoStatement((BoundGotoStatement)node);
                    }
                case BoundNodeKind.LabelStatement:
                    {
                        return RewriteLabelStatement((BoundLabelStatement)node);
                    }
                case BoundNodeKind.ConditionalGotoStatement:
                    {
                        return RewriteConditionalGotoStatement((BoundConditionalGotoStatement)node);
                    }
                default:
                    throw new Exception($"Unexpected node kind. BoundNodeKind:{node.Kind}");
            }
        }

        public BoundExpression RewriteExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.UnaryExpression:
                    {
                        return RewriteUnaryExpression(((BoundUnaryExpression)node));
                    }
                case BoundNodeKind.LiteralExpression:
                    {
                        return RewriteLiteralExpression(((BoundLiteralExpression)node));
                    }
                case BoundNodeKind.BinaryExpression:
                    {
                        return RewriteBinaryExpression(((BoundBinaryExpression)node));
                    }
                case BoundNodeKind.VariableExpression:
                    {
                        return RewriteVariableExpression(((BoundVariableExpression)node));
                    }
                case BoundNodeKind.AssignmentExpression:
                    {
                        return RewriteAssignmentExpression(((BoundAssignmentExpression)node));
                    }
                case BoundNodeKind.BoundErrorExpression:
                    {
                        return node;
                    }
                default:
                    throw new Exception($"Unexpected node kind. BoundNodeKind:{node.Kind}");
            }
        }

        protected virtual BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            var condition = RewriteExpression(node.Condition);

            if (condition != node.Condition)
            {
                return new BoundConditionalGotoStatement(node.Label, condition, node.JumpIf);
            }

            return node;
        }

        protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteGotoStatement(BoundGotoStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var lowerBound = RewriteExpression(node.LowerBound);
            var upperBound = RewriteExpression(node.UpperBound);
            var statement = RewriteStatement(node.Statement);

            if (lowerBound != node.LowerBound)
            {
                return Create();
            }

            if (upperBound != node.UpperBound)
            {
                return Create();
            }

            if (statement != node.Statement)
            {
                return Create();
            }

            return node;

            BoundForStatement Create() => new BoundForStatement(node.Variable, lowerBound, upperBound, statement);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var statement = RewriteStatement(node.Statement);

            if (condition != node.Condition)
            {
                return Create();
            }

            if (statement != node.Statement)
            {
                return Create();
            }

            return node;

            BoundWhileStatement Create() => new BoundWhileStatement(condition, statement);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var thenStatement = RewriteStatement(node.Statement);
            var elseStatement = node.ElseClause is null ? null : RewriteStatement(node.ElseClause);

            if (condition != node.Condition)
            {
                return Create();
            }

            if (thenStatement != node.Statement)
            {
                return Create();
            }

            if (elseStatement != node.ElseClause)
            {
                return Create();
            }

            return node;

            BoundIfStatement Create() => new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            var initializer = RewriteExpression(node.Initializer);

            if (initializer != node.Initializer)
            {
                return new BoundVariableDeclaration(node.Variable, initializer);
            }

            return node;
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression != node.Expression)
            {
                return new BoundExpressionStatement(expression);
            }

            return node;
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = null;

            for (int i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(oldStatement);

                if (newStatement != oldStatement)
                {
                    if (builder is null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);

                        for (int j = 0; j < i; j++)
                        {
                            builder.Add(node.Statements[j]);
                        }
                    }
                }

                if (builder != null)
                {
                    builder.Add(newStatement);
                }
            }

            if (builder is null)
            {
                return node;
            }

            return new BoundBlockStatement(builder.MoveToImmutable());
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression != node.Expression)
            {
                return new BoundAssignmentExpression(node.Variable, expression);
            }

            return node;
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var left = RewriteExpression(node.Left);
            var right = RewriteExpression(node.Right);

            if (left != node.Left)
            {
                return Create();
            }

            if (right != node.Right)
            {
                return Create();
            }

            return node;

            BoundBinaryExpression Create() => new BoundBinaryExpression(left, node.Op, right);
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var operand = RewriteExpression(node.Operand);
            if (operand != node.Operand)
            {
                return new BoundUnaryExpression(node.Op, operand);
            }

            return node;
        }
    }
}