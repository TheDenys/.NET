using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace NET4.TestClasses
{
    [Serializable]
    public class ROColllection<T> : NameObjectCollectionBase
    {

        public ROColllection(bool bReadOnly)
        {
            IsReadOnly = bReadOnly;
        }

        public ROColllection(IDictionary<string, T> dict, bool bReadOnly)
        {
            foreach (KeyValuePair<string, T> pair in dict)
            {
                this.BaseAdd((String) pair.Key, pair.Value);
            }
            IsReadOnly = bReadOnly;
        }

        public ROColllection(ROColllection<T> coll, bool bReadOnly)
        {
            foreach (string key in coll)
            {
                this.BaseSet(key, coll[key]);
            }
            IsReadOnly = bReadOnly;
        }

        public T this[string key]
        {
            get
            {
                return (T) BaseGet(key);
            }
            set
            {
                BaseSet(key, value);
            }
        }

        public void Add(String key, T value)
        {
            BaseAdd(key, value);
        }

    }
}
