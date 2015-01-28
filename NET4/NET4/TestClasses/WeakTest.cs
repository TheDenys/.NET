using System;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class WeakTest
    {
        [Run(0)]
        protected void DoWeakRef()
        {
            WeakReference wr = new WeakReference(new WRClass { data = "123" });
            GC.Collect();
            ConsolePrint.print("wr:{0}m, wr.isAlive:{1} wr.target:{2}", wr, wr.IsAlive, wr.Target);
        }

        private class WRClass
        {
            public string data;
        }
    }
}