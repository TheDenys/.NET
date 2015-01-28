using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace AmbNs
{
    [RunableClass]
    public class AmbiguosClass
    {

        public static string REAL_NAME { get { return "ClassLibraryTest"; } }

        [Run(0)]
        public void Foo()
        {
            ConsolePrint.print("CLTest2");
        }

    }
}
