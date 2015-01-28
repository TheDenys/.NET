using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.PlayWithHash
{

    [RunableClass]
    public class UseHashMap
    {
        [Run(0)]
        protected static void UseFixedSizeHashMap()
        {
            FixedSizeHasMap<int,int> map = new FixedSizeHasMap<int, int>(5);
            map[0] = 1;
            map[1] = 3;
            map[4] = 12;
            map[-4] = 2;
            map[17] = 22;
            ConsolePrint.print(map);

            map.Remove(4);
            ConsolePrint.print(map);
        }
    }
}