using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Syntax.Binding
{
    internal abstract class BoundNode
    {
        public BoundNode()
        {
        }

        public abstract BoundNodeKind Kind { get; }

        public virtual IEnumerable<BoundNode> GetChildren()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);
                    if (child is null)
                    {
                        continue;
                    }

                    yield return child;
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);

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

        public void WriteTo(TextWriter writer)
        {
            PrintTree(writer, this);
        }

        private static void PrintTree(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);

            var text = GetText(node);
            writer.Write(text);

            var isFirst = true;
            foreach (var item in node.GetProperties())
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.Write(",");
                }

                writer.Write($" {item.Name} : {item.Value}");

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

        private static string GetText(BoundNode node)
        {
            // TODO (R) Bug EP10: >for throws error because of Op is null.
            switch (node)
            {
                case BoundBinaryExpression expression:
                    return expression.Op.Kind.ToString() + "Expression";
                case BoundUnaryExpression expression:
                    return expression.Op.Kind.ToString() + "Expression";
                default:
                    return node.Kind.ToString();
            }
        }

        private IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (string.Equals(property.Name, nameof(Kind)) || string.Equals(property.Name, nameof(BoundBinaryExpression.Op)))
                {
                    continue;
                }
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                var value = property.GetValue(this);
                if (value != null)
                {
                    if(value is Type t)
                    {
                        value = t.Name;
                    }

                    if(value is VariableSymbol v)
                    {
                        value = v.Name;
                    }

                    if(value is LabelTag l)
                    {
                        value = l.Name;
                    }

                    yield return (property.Name, value);
                }
            }
        }
    }
}