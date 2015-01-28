using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    class P001_Multiples_3_5
    {
                
        [Run(0)]
        protected void GetSum(){
            long sum = 0;

            for (int i = 1; i < 1000; i++)
            {
                if (i % 3 == 0 || i % 5 == 0)
                    sum += i;
            }

            ConsolePrint.print("sum={0}",sum);
        }

    }
}
