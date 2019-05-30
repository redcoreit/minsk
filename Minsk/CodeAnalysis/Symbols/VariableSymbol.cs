using System;

namespace Minsk.CodeAnalysis.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type, bool isCompilerGenerated = false) 
            : base(isCompilerGenerated ? $"<>_{name}" : name)
        {
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public bool IsReadOnly { get; }
        public Type Type { get; }
        public override SymbolKind Kind => SymbolKind.Variable;
    }
}
