using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.EffectiveCSharp
{

    internal class MySimpleClass
    {
        internal MySimpleClass(int value)
        {
            this.value = value;
        }

        private int value;

        internal int Value
        {
            get { return value; }
        }
    }

    internal class MySecondSimpleClass
    {
        internal MySecondSimpleClass(int value)
        {
            this.value = value;
        }

        private int value;

        internal int Value
        {
            get { return value; }
        }
    }

    internal class MyClassWithCast
    {
        internal MyClassWithCast(int value)
        {
            this.value = value;
        }

        private int value;

        internal int Value
        {
            get { return value; }
        }

        public static implicit operator MySimpleClass(MyClassWithCast m)
        {
            return new MySimpleClass(m.Value);
        }

        public static explicit operator MySecondSimpleClass(MyClassWithCast m)
        {
            return new MySecondSimpleClass(m.Value);
        }
    }

    internal class Base
    {
        public Base(int value)
        {
            this.value = value;
        }

        protected readonly int value;

        internal virtual int Value
        {
            get { return value; }
        }
    }

    internal class Derived : Base
    {
        public Derived(int value) : base(value) { }

        internal int Value
        {
            get { return value + 100; }
        }
    }

    [RunableClass]
    internal class TryAsIsCast
    {

        [Run(0)]
        protected void TryCast()
        {
            MyClassWithCast mcwc = new MyClassWithCast(28);

            // implicit cast
            MySimpleClass msc = mcwc;

            // explicit cast
            MySecondSimpleClass mssc = (MySecondSimpleClass)mcwc;
        }

        [Run(0)]
        protected void TryForeachCast()
        {
            IEnumerable coll = new ArrayList(
                new MyClassWithCast[]
                    {
                        new MyClassWithCast(10),
                        new MyClassWithCast(15),
                        new MyClassWithCast(30),
                    });

            // works just fine
            foreach (MyClassWithCast var in coll)
            {
                Console.WriteLine("value:{0}", var.Value);
            }

            try
            {
                // thrws invalidcast
                foreach (MySimpleClass var in coll)
                {
                    Console.WriteLine("value:{0}", var.Value);
                }

            }
            catch (InvalidCastException invalidCastException)
            {
                ConsolePrint.print(invalidCastException);
            }

            foreach (MySimpleClass var in coll.Cast<MyClassWithCast>())
            {
                Console.WriteLine("value:{0}", var.Value);
            }
        }

        [Run(0)]
        protected void TryAsIs()
        {
            object o = new Base(1);
            bool isBase = o is Base;
            ConsolePrint.print("isBase:{0}", isBase);
            Base b = o as Base;
            bool isDerived = b is Derived;
            ConsolePrint.print("isDerived:{0}", isDerived);

            object o2 = new Derived(2);
            isBase = o2 is Base;
            ConsolePrint.print("isBase:{0}", isBase);
            isDerived = o2 is Derived;
            ConsolePrint.print("isDerived:{0}", isDerived);

            Base b2 = o2 as Derived;
            ConsolePrint.print("value:{0}", b2.Value);// 2

            Derived d2 = o2 as Derived;
            ConsolePrint.print("value:{0}", d2.Value);// 102
        }
    }
}