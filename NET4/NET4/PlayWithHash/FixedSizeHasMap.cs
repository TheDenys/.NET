using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NET4.PlayWithHash
{
    public class FixedSizeHasMap<TK, TV>
    {
        private const int DEFAULT_SIZE = 10;

        private int size;

        private LinkedList<KeyValue<TK, TV>>[] items;

        public FixedSizeHasMap() : this(DEFAULT_SIZE) { }

        public FixedSizeHasMap(int size)
        {
            this.size = size;
            this.items = new LinkedList<KeyValue<TK, TV>>[size];
        }

        private int GetPosition(TK key)
        {
            int position = key.GetHashCode() % size;
            return Math.Abs(position);
        }

        private LinkedList<KeyValue<TK, TV>> GetBucket(int position)
        {
            LinkedList<KeyValue<TK, TV>> linkedList = items[position];

            if (linkedList == null)
            {
                linkedList = new LinkedList<KeyValue<TK, TV>>();
                items[position] = linkedList;
            }

            return linkedList;
        }

        public TV this[TK key]
        {
            get
            {
                int pos = GetPosition(key);
                LinkedList<KeyValue<TK, TV>> linkedList = GetBucket(pos);

                foreach (var keyValue in linkedList)
                {
                    if (object.Equals(keyValue.Key, key))
                    {
                        return keyValue.Value;
                    }
                }

                return default(TV);
            }

            set
            {
                int pos = GetPosition(key);
                LinkedList<KeyValue<TK, TV>> bucket = GetBucket(pos);
                bucket.AddLast(new KeyValue<TK, TV>(key, value));
            }
        }

        public bool Remove(TK key)
        {
            int pos = GetPosition(key);
            LinkedList<KeyValue<TK, TV>> linkedList = GetBucket(pos);

            bool found = false;
            KeyValue<TK, TV> keyValueFound = new KeyValue<TK, TV>();

            foreach (KeyValue<TK, TV> keyValue in linkedList)
            {
                if (object.Equals(keyValue.Key, key))
                {
                    found = true;
                    keyValueFound = keyValue;
                }
            }

            if (found)
            {
                linkedList.Remove(keyValueFound);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[");

            foreach (var linkedList in items)
            {
                if (linkedList != null)
                {
                    foreach (var keyValue in linkedList)
                    {
                        sb.AppendFormat("{{{0}:{1}}},", keyValue.Key, keyValue.Value);
                    }
                }
            }

            sb.Length--;
            sb.Append("]");
            return sb.ToString();
        }

        private struct KeyValue<K, V>
        {
            private K key;
            private V value;

            public K Key { get { return key; } }

            public V Value { get { return value; } }

            public KeyValue(K key, V value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }
}