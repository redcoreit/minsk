using System;

namespace Minsk.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Int = new TypeSymbol("int");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol String = new TypeSymbol("string");
        public static readonly TypeSymbol Error = new TypeSymbol("?");

        private TypeSymbol(string name) : base(name)
        {
        }

        internal static TypeSymbol FromClrType(object value)
        {
            switch (value)
            {
                case bool _: return Bool;
                case int _: return Int;
                case string _: return String;
                case null:
                    throw new ArgumentNullException(nameof(value));
                default:
                    throw new NotSupportedException($"Clr type not supported by Minsk type system. ClrType: {value.GetType().FullName}");
            }
        }

        public override SymbolKind Kind => SymbolKind.Type;
    }
}
