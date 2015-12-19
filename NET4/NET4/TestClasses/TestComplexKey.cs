using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestComplexKey : RunableBase
    {
        private string[] arr;

        [Run(0)]
        public void TestSome()
        {
            arr = new string[] { };
            DebugFormat("arr: {0}, hashL {1}.", arr, arr.GetHashCode());
            arr = new string[] { };
            DebugFormat("arr: {0}, hashL {1}.", arr, arr.GetHashCode());
            arr = new[] { "1" };
            DebugFormat("arr: {0}, hashL {1}.", arr, arr.GetHashCode());
            arr = new[] { "1", "2" };
            DebugFormat("arr: {0}, hashL {1}.", arr, arr.GetHashCode());
            arr = new[] { "1", "2", "3" };
            DebugFormat("arr: {0}, hashL {1}.", arr, arr.GetHashCode());
            arr = new[] { "1", "3" };
            DebugFormat("arr: {0}, hashL {1}.", arr, arr.GetHashCode());

            var listEqualityComparer = new IListEqualityComparer<string>();
            IDictionary<string[], LinkedList<string[]>> dictLeft = new Dictionary<string[], LinkedList<string[]>>(listEqualityComparer);
            IDictionary<string[], LinkedList<string[]>> dictRight = new Dictionary<string[], LinkedList<string[]>>(listEqualityComparer);

            var entriesLeft = new[]
            {
                new Entry(new []{"k","1"},new []{"v1"}),
                new Entry(new []{"k","2"},new []{"v2"}),
                new Entry(new []{"k","3"},new []{"v3"}),
                new Entry(new []{"k","4"},new []{"v4"}),
                new Entry(new []{"k","5"},new []{"v4"}),
                new Entry(new []{"k","5"},new []{"v4"}),
            };

            var entriesRight = new[]
            {
                new Entry(new []{"k","1"},new []{"v1"}),
                new Entry(new []{"k","2"},new []{"v2"}),
                new Entry(new []{"k","3"},new []{"v3"}),
                new Entry(new []{"k","4"},new []{"v4"}),
            };

            FillDict(entriesLeft, dictLeft);
            FillDict(entriesRight, dictRight);

            var set = new HashSet<string[]>(listEqualityComparer);
            var added = set.Add(new string[] { "1" });
            added = set.Add(new string[] { "1" });
        }

        void FillDict(IEnumerable<Entry> entries, IDictionary<string[], LinkedList<string[]>> dict)
        {
            foreach (var entry in entries)
            {
                LinkedList<string[]> val;
                if (dict.TryGetValue(entry.Keys, out val))
                {
                    val.AddLast(entry.Values);
                }
                else
                {
                    dict.Add(new KeyValuePair<string[], LinkedList<string[]>>(entry.Keys, new LinkedList<string[]>(Enumerable.Repeat(entry.Values, 1))));
                }
            }
        }

        internal class Entry
        {
            readonly string[] _keys;
            readonly string[] _values;

            public Entry(string[] keys, string[] values)
            {
                this._keys = keys;
                this._values = values;
            }

            public string[] Keys { get { return _keys; } }
            public string[] Values { get { return _values; } }
        }
    }
}