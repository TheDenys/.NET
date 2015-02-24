using System;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.EffectiveCSharp
{

    [RunableClass]
    internal class TrumEnum : RunableBase
    {
        private enum SomeEnum
        {
            Foo = 2,
            Bar = 4
        }

        private enum DefEnum
        {
            Nop = 0,
            Foo = 1,
            Bar = 2,
            BooFar = 4
        }

        [Run(1)]
        protected void FromString()
        {
            SomeEnum foo = SomeEnum.Foo;
            string val = foo.ToString();
            DebugFormat("val={0}.", val);
            var tmp = (SomeEnum)Enum.Parse(typeof(SomeEnum), val, true);
            DebugFormat("obj={0}", tmp);
            DebugFormat("eq={0}", foo == tmp);
        }

        [Run(0)]
        protected static void TestEnums()
        {
            SomeEnum broken = new SomeEnum();// the same as broken = 0;
            ConsolePrint.print("broken:{0}", broken);

            broken = 0;
            ConsolePrint.print("broken:{0}", broken);

            broken = (SomeEnum)(-10);
            ConsolePrint.print("broken:{0}", broken);

            var vals = Enum.GetValues(typeof(DefEnum));
            ConsolePrint.print(vals);

            foreach (var val in vals)
            {
                ConsolePrint.printMap(val.ToString(), ((int)val).ToString());
            }

            ConsolePrint.print("enum value: {0}", Utils.GetEnumObject<SomeEnum>("Bar"));

            SomeEnum se = SomeEnum.Bar | SomeEnum.Foo;
            bool ebul = ((se & SomeEnum.Bar) == SomeEnum.Bar);

            ConsolePrint.print("enum item:[" + se.ToString() + "]");

            DefEnum denum = DefEnum.Nop;
            denum = denum | DefEnum.Foo;
            denum = denum | DefEnum.Bar;

            if ((denum & DefEnum.Foo) == DefEnum.Foo)
            {
                ConsolePrint.print("enum item:[" + DefEnum.Foo.ToString() + "]");
            }

            if ((denum & DefEnum.Bar) == DefEnum.Bar)
            {
                ConsolePrint.print("enum item:[" + DefEnum.Bar.ToString() + "]");
            }

            if ((denum & DefEnum.BooFar) == DefEnum.BooFar)
            {
                ConsolePrint.print("enum item:[" + DefEnum.BooFar.ToString() + "]");
            }

            ConsolePrint.print("enum item:[" + denum.ToString() + "]");
        }
    }

}