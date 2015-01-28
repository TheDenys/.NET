using System;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.EffectiveCSharp
{
    [RunableClass]
    public class BoxingUnboxing
    {
        
        [Run(0)]
        private void BoxUnbox()
        {

            object o1 = 1;
            int i1 = (int) o1;

            ConsolePrint.print("o1={0}", o1);
        }

    }
}