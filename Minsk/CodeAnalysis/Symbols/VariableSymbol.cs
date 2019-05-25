using System;

namespace Minsk.CodeAnalysis.Symbols
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type, bool isCompilerGenerated = false)
        {
            Name = isCompilerGenerated ? $"<>_{name}" : name;
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public string Name { get; }
        public bool IsReadOnly { get; }
        public Type Type { get; }
    }
}
