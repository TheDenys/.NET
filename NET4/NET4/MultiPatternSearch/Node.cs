using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NET4.MultiPatternSearch
{
    public class NodesTree
    {
        public static Node BuildRootNode(params Node[] childNodes)
        {
            // root node is just a string node with empty string pattern
            var node = StringNode.BuildRootNode();

            if (childNodes?.Length > 0)
            {
                node.Nodes.AddRange(childNodes);
            }

            return node;
        }

        public static Node BuildWildCardNode(string name, params Node[] childNodes) => BuildWildCardNode(name, false, childNodes);

        public static Node BuildWildCardNode(string name, bool canTerminate, params Node[] childNodes)
        {
            var node = new WildcardNode(name) { CanTerminate = canTerminate };

            var hasChildWildcard = childNodes.OfType<WildcardNode>().Any();

            if (hasChildWildcard)
            {
                // patterns like "a*b*" make sense, but "a**b" or "**" make no sense, so we prohibit adjacent wildcards
                throw new ArgumentException("Can not add wildcard node to another wildcard node.");
            }

            if (childNodes?.Length > 0)
            {
                node.Nodes.AddRange(childNodes);
            }

            return node;
        }

        public static Node BuildStringNode(string value, params Node[] childNodes) => BuildStringNode(value, false, childNodes);

        public static Node BuildStringNode(string value, bool canTerminate, params Node[] childNodes)
        {
            var node = new StringNode(value) { CanTerminate = canTerminate };

            if (childNodes?.Length > 0)
            {
                node.Nodes.AddRange(childNodes);
            }

            return node;
        }

        public abstract class Node
        {
            public string PatternOrigin { get; set; }
            public int Order { get; set; }

            private bool canTerminate;

            /// <summary>
            /// User can explicitly make node terminating to allow match both patterns like: "a*" and "a*c"
            /// </summary>
            public bool CanTerminate
            {
                get { return canTerminate || Nodes.Count == 0; }
                set { canTerminate = value; }
            }

            public virtual bool IsWildcard { get; }
            public string Value { get; set; }
            public List<Node> Nodes { get; } = new List<Node>();

            protected Node(string value)
            {
                Value = value;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{ ");
                sb.Append(IsWildcard ? ("%" + Value + "%") : "[" + Value + "]");
                sb.Append(" ");

                foreach (var node in Nodes)
                {
                    sb.Append(node.ToString());
                }

                sb.Append("}");

                return sb.ToString();
            }
        }

        private class StringNode : Node
        {
            public override bool IsWildcard => false;

            // can be used by NodeFactory only
            private StringNode() : base("") { }

            public StringNode(string value) : base(value)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"Value must contain at least one non-space character.", nameof(value));
            }

            public static Node BuildRootNode() => new StringNode();
        }

        private class WildcardNode : Node
        {
            public WildcardNode(string name) : base(name)
            {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"Value must contain at least one non-space character.", nameof(name));
            }

            public override bool IsWildcard => true;
        }
    }

}
