using System.Diagnostics;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestDebuggerLaunch
    {
        [Run(0)]
        protected void Launch()
        {
            ConsolePrint.print("attached: {0}", Debugger.IsAttached);
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            else
            {
                Debugger.Break();
            }
        }
    }
}