using System;
using PDNUtils.Help;

namespace DirtyLib
{
    [Obsolete("obsolete!", false)]
    public class ObsoleteClass
    {
        public static void Do()
        {
            ConsolePrint.print("obsolete true!!!");
        }
    }
}
