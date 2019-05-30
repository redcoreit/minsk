using System;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class LabelTag
    {
        internal LabelTag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
