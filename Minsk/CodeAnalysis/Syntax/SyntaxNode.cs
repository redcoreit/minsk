using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().FirstOrDefault();
                var last = GetChildren().LastOrDefault();

                if (first is null)
                {
                    throw new Exception($"GetChildren() must return at least one element.");
                }

                return TextSpan.FromBounds(first.Span.Start, last.Span.End);

            }
        }

        public virtual IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);
                    if (child is null)
                    {
                        continue;
                    }

                    yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);

                    if (children is null)
                    {
                        continue;
                    }

                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
            {
                return token;
            }

            return GetChildren().Last().GetLastToken();
        }

        public void WriteTo(TextWriter writer)
        {
            PrintTree(writer, this);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                PrintTree(writer, this);
                return writer.ToString();
            }
        }

        private static void PrintTree(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            writer.WriteLine();

            indent += isLast ? "   " : "|  ";
            var lastChild = node.GetChildren().LastOrDefault();

            using (var enumerator = node.GetChildren().GetEnumerator())
            {
                var hasNext = enumerator.MoveNext();
                while (hasNext)
                {
                    PrintTree(writer, enumerator.Current, indent, !(hasNext = enumerator.MoveNext()));
                }
            }
        }
    }
}
