using System.Collections.Generic;
using PDNUtils.Compare;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestDictionary
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Run(false)]
        protected static void DictionaryTest()
        {
            //IEqualityComparer<object[]> comparer = new ObjectArrayComparer();
            //var d = new Dictionary<object[], string>(comparer);
            //var foo = new object[] { "1", "2" };
            //var bar = new object[] { "1", "2" };
            //var foobar = new object[] { "3", "1" };
            //var barfoo = new object[] { "3", "1xxxccc" };
            //d[foo] = "foo";
            //d[bar] = "bar";
            //d[foobar] = "foobar";
            //d[barfoo] = "barfoo";



            //IEqualityComparer<string[]> comparer = new StringArrayComparer();
            //var d = new Dictionary<string[], string>(comparer);
            //var foo = new string[] { "1", "2xcxcxc" };
            //var bar = new string[] { "1", "2" };
            //var foobar = new string[] { "3", "1" };
            //var barfoo = new string[] { "3", "1" };
            //d[foo] = "foo";
            //d[bar] = "bar";
            //d[foobar] = "foobar";
            //d[barfoo] = "barfoo";

            var netObjects = CreateNetObjects();
            var relatedEntityKeysDictionary = getRelatedEntityKeysDictionary();

            var res = CreateNetObjectMappings(relatedEntityKeysDictionary, netObjects);


            //ConsolePrint.print(d);
        }

        private static IDictionary<string, IDictionary<object[], NetObject>> CreateNetObjectMappings(IEnumerable<KeyValuePair<string, string[]>> relatedEntityKeysDictionary, IEnumerable<NetObject> netObjects)
        {
            var result = new Dictionary<string, IDictionary<object[], NetObject>>();
            var keysCache = new Dictionary<string[], IDictionary<object[], NetObject>>(new StringArrayComparer());
            foreach (var keyValuePair in relatedEntityKeysDictionary)
            {
                var keySet = keyValuePair.Value;
                IDictionary<object[], NetObject> mapping;
                if (!keysCache.TryGetValue(keySet, out mapping))
                {
                    mapping = MakeMapping(keySet, netObjects);
                    keysCache[keySet] = mapping;
                }
                result.Add(keyValuePair.Key, mapping);
            }
            return result;
        }

        private static Dictionary<object[], NetObject> MakeMapping(string[] keySet, IEnumerable<NetObject> netObjects)
        {
            var res = new Dictionary<object[], NetObject>(new ObjectArrayComparer());
            foreach (var netObject in netObjects)
            {
                object[] key = GenerateKey(keySet, netObject);
                if (key != null)
                {
                    res[key] = netObject;
                }
            }
            return res;
        }

        private static object[] GenerateKey(string[] keySet, NetObject netObject)
        {
            var res = new object[keySet.Length];
            int cout = 0;

            foreach (var keyName in keySet)
            {
                var dictionary = netObject.RelatedEntityRow.Properties;
                object value;
                if (dictionary.TryGetValue(keyName, out value))
                {
                    res[cout++] = value;
                }
                else
                {
                    //TODO probably throw an exception
                    log.WarnFormat("there is no property with name '{0}'", keyName);
                    return null;
                }
            }

            return res;
        }

        private static Dictionary<string, string[]> getRelatedEntityKeysDictionary()
        {
            return new Dictionary<string, string[]>()
                       {
                           {"Interface",new string[]{"id"}},
                           {"Volume",new string[]{"id"}},
                           {"Component",new string[]{"id1","id2"}},
                       };
        }

        private static IEnumerable<NetObject> CreateNetObjects()
        {
            return new[]
                       {
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Node 1"},
                                                        {"id",19},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Node 2"},
                                                        {"id",19},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Node 3"},
                                                        {"id",199},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Node 21"},
                                                        {"id",219},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Node 22"},
                                                        {"id",2190},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Comp 1"},
                                                        {"id1",190},
                                                        {"id2",2190},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Comp 2"},
                                                        {"id1",190},
                                                        {"id2",2190},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Comp 3"},
                                                        {"id1",9190},
                                                        {"id2",2190},
                                                    }
                               },
                           new NetObject
                               {
                                   Properties = new Dictionary<string,object>
                                                    {
                                                        {"name","Comp 4"},
                                                        {"id1",9190},
                                                        {"id2",2190},
                                                    }
                               },
                       };
        }

        public class NetObject
        {
            public ManagedEntity relatedEntityRow = new ManagedEntity();
            public ManagedEntity RelatedEntityRow
            {
                get
                {
                    return relatedEntityRow;
                }
            }

            public IDictionary<string, object> Properties
            {
                set
                {
                    foreach (var keyValuePair in value)
                    {
                        relatedEntityRow.Properties.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }
        }

        public class ManagedEntity
        {
            private Dictionary<string, object> properties = new Dictionary<string, object>();
            public Dictionary<string, object> Properties
            {
                get
                {
                    return properties;
                }
            }
        }

    }

}