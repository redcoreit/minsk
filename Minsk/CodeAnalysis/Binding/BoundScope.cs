using System.Collections.Generic;
using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal sealed class BoundScope
    {
        private readonly Dictionary<string, VariableSymbol> _variables;

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
            _variables = new Dictionary<string, VariableSymbol>();
        }

        public BoundScope Parent { get; }

        public bool TryDeclare(VariableSymbol variable)
        {
            // TODO: should we support shadowing?

            if(_variables.ContainsKey(variable.Name))
            {
                return false;
            }

            _variables.Add(variable.Name, variable);
            
            return true;
        }

        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if(_variables.TryGetValue(name, out variable))
            {
                return true;
            }

            if(Parent is null)
            {
                return false;
            }

            return Parent.TryLookup(name, out variable);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => _variables.Values.ToImmutableArray(); 
    }
}