using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Minsk.CodeAnalysis.Lowering;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Syntax.Binding;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope _globalScope;

        public Compilation(SyntaxTree syntaxTree)
            : this(null, syntaxTree)
        {
        }

        private Compilation(Compilation previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        public Compilation Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope is null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree) => new Compilation(this, syntaxTree);

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var statement = GetStatement();
            var evaluator = new Evaluator(statement, variables);

            var diag = new DiagnosticBag();
            diag.AddRange(SyntaxTree.Diagnostics);
            diag.AddRange(_globalScope.Diagnostics);

            return new EvaluationResult(diag.ToImmutableArray(), diag.Any() ? null : evaluator.Evaluate());
        }

        public void WriteTreeTo(TextWriter writer)
        {
            var statement = GetStatement();
            statement.WriteTo(writer);
        }

        private BoundBlockStatement GetStatement() => Lowerer.Lower(GlobalScope.Statement);
    }
}
