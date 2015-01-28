using System;
using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;
using System.Runtime.Caching;

namespace NET4.TestClasses
{
    [RunableClass]
    public class WorkWithCache
    {
        [Run(0)]
        protected void UseMemoryCache()
        {
            MemoryCache mc = new MemoryCache("mycache");
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(10d);
            mc.Add("k1", "123", cacheItemPolicy);

            ConsolePrint.print("{0} {1} {2}", mc.Contains("k1"), mc.GetCount(), mc.Get("k1"));

            Thread.Sleep(15000);

            ConsolePrint.print("{0} {1} {2}", mc.Contains("k1"), mc.GetCount(), mc.Get("k1"));
        }
    }
}