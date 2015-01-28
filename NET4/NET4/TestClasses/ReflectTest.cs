using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class ReflectTest
    {

        public const string weird_name = "@#$EWR-===/*--+awwe";
        public const string weird_nameXX = "@#$EWxcvcxvR-===/*--+awwe";
        public const string weird_nameWW = "@#$EWR-===/*--+awwxcvxcve";

        [Run(false)]
        public static void print()
        {

            object[] vals = ReflectionHelper.GetConstantValues<ReflectTest>();

            foreach (object val in vals)
            {
                Console.Out.WriteLine("val = {0}", val);
            }

        }

        [Run(false)]
        public static void Go()
        {
            object o = new ReflectTestClass();
            object o1 = new ReflectTestClass();
            Type t = o.GetType();
            PropertyInfo pi = t.GetProperty("A");
            pi.SetValue(o, 99, null);
            pi.SetValue(o1, 199, null);
            int val = ((ReflectTestClass)o).A;
            int val1 = ((ReflectTestClass)o1).A;
            ConsolePrint.print("val=" + val + " val1=" + val1);

            var propFromReflection = pi.GetValue(o, BindingFlags.GetProperty, null, null, null);
            ConsolePrint.print("from reflection : {0}", propFromReflection);
        }

        [Run(false)]
        protected static void CompareProperties()
        {
            var o1 = new ReflectTestClass { A = 1, B = 2, C = 3 };
            var o2 = new ReflectTestClass { A = 1, B = 2, C = 3 };
            string message;

            var eq = InnerCompareProperties(o1, o2, null, out message);
            ConsolePrint.print("o1==o2: {0}, message: {1}", eq, message);

            o1.C = 4;
            eq = InnerCompareProperties(o1, o2, null, out message);
            ConsolePrint.print("o1==o2: {0}, message: {1}", eq, message);

            eq = InnerCompareProperties(o1, o2, new[] { "C" }, out message);
            ConsolePrint.print("o1==o2: {0}, message: {1}", eq, message);
        }

        private static bool InnerCompareProperties<T>(T o1, T o2, IEnumerable<string> ignoredNames, out string message) where T : class
        {
            IEnumerable<PropertyInfo> notEq;
            var eq = ReflectionHelper.CompareInstanceProperties(o1, o2, ignoredNames, out notEq);
            message = !eq ? string.Format("Next properties are not equal on objects: [{0}]", string.Join(",", notEq.Select(p => string.Format(@"""{0}""", p.Name)))) : null;
            return eq;
        }

        [Run(0)]
        public void ImitateConstructor()
        {
            var ip = (IPAddress)Activator.CreateInstance(typeof (IPAddress));
        }
    }

    public class ReflectTestClass
    {
        int a;
        public int A
        {
            get { return a; }
            set { a = value; }
        }

        int b;
        public int B
        {
            get { return b; }
            set { b = value; }
        }

        int c;
        public int C
        {
            get { return c; }
            set { c = value; }
        }

    }

}
