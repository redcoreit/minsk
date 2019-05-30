using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Syntax.Binding;

namespace Minsk.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount;

        private Lowerer()
        {
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            var result = lowerer.RewriteStatement(statement);

            return Flatten(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            /* 
                {
                    let upperBound = <UpperBound>
                    <variable> = <lowerBound>
                    while <variable> < <UpperBound>
                    {
                        <statement>
                        <variable> = <variable> + 1
                    }
                }
            */

            var upperBoundSymbol = new VariableSymbol("upperBound", true, node.UpperBound.Type, true);
            var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
            var upperBoundExpression = new BoundVariableExpression(upperBoundSymbol);
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound); // LowerBound shouldn't be rewriten?
            var variableExpression = new BoundVariableExpression(node.Variable);
            var conditionOperator = BoundBinaryOperator.Bind(SyntaxKind.LessToken, node.Variable.Type, node.UpperBound.Type);
            var condition = new BoundBinaryExpression(variableExpression, conditionOperator, upperBoundExpression); // UpperBound shouldn't be rewriten?

            var incrementOperator = BoundBinaryOperator.Bind(SyntaxKind.PlusToken, node.Variable.Type, TypeSymbol.Int);
            var increment = new BoundBinaryExpression(variableExpression, incrementOperator, new BoundLiteralExpression(1));
            var incrementAssignment = new BoundAssignmentExpression(node.Variable, increment);
            var incrementStatement = new BoundExpressionStatement(incrementAssignment);

            var whileBlockStatement = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Statement, incrementStatement));
            var whileStatement = new BoundWhileStatement(condition, whileBlockStatement);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(upperBoundDeclaration, variableDeclaration, whileStatement));

            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            /*
                gotoIfFalse <condition> end
                <Then>
                end:

                gotoIfFalse <condition> else
                <Then>
                goto end
                else:
                <Else>
                end:
             */

            BoundBlockStatement result;
            if (node.ElseClause is null)
            {
                result = Create();
            }
            else
            {
                result = CreateWithElse();
            }

            return RewriteStatement(result);

            BoundBlockStatement Create()
            {
                var endLabelStatement = new BoundLabelStatement(GenerateLabel());
                var gotoIfFalseStatement = new BoundConditionalGotoStatement(endLabelStatement.Label, node.Condition, false);
                return new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(gotoIfFalseStatement, node.Statement, endLabelStatement));
            }

            BoundBlockStatement CreateWithElse()
            {
                var elseLabelStatement = new BoundLabelStatement(GenerateLabel());
                var endLabelStatement = new BoundLabelStatement(GenerateLabel());
                var gotoIfFalseStatement = new BoundConditionalGotoStatement(elseLabelStatement.Label, node.Condition, false);
                var gotoEndStatement = new BoundGotoStatement(endLabelStatement.Label);
                var statements = ImmutableArray.Create<BoundStatement>(gotoIfFalseStatement, node.Statement, gotoEndStatement, elseLabelStatement, node.ElseClause, endLabelStatement);
                return new BoundBlockStatement(statements);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            /*
                check:
                gotoIfFalse <condition> end
                <statement>
                goto check
                end:
             */

            var checkLabelStatement = new BoundLabelStatement(GenerateLabel());
            var endLabelStatement = new BoundLabelStatement(GenerateLabel());
            var gotoIfFalseStatement = new BoundConditionalGotoStatement(endLabelStatement.Label, node.Condition, false);
            var gotoCheckStatement = new BoundGotoStatement(checkLabelStatement.Label);
            var statements = ImmutableArray.Create<BoundStatement>(checkLabelStatement, gotoIfFalseStatement, node.Statement, gotoCheckStatement, endLabelStatement);
            var result = new BoundBlockStatement(statements);

            return RewriteStatement(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement node)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            var stack = new Stack<BoundStatement>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current is BoundBlockStatement block)
                {
                    foreach (var child in block.Statements.Reverse())
                    {
                        stack.Push(child);
                    }
                }
                else
                {
                    statements.Add(current);
                }
            }

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private LabelTag GenerateLabel() => new LabelTag($"Label{_labelCount++}");
    }
}