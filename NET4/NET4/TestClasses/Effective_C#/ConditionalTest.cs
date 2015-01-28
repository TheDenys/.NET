using System.Diagnostics;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.EffectiveCSharp
{
    [RunableClass]
    internal class TestConditional
    {
        [Run(0)]
        protected void CallConditional()
        {
            CalledIfDebug();
            CalledIfDebugOrTrace();
            CalledIfTrace();
        }

        [Conditional("DEBUG")]
        private void CalledIfDebug()
        {
            ConsolePrint.print("this is called on DEBUG only.");
        }

        [Conditional("TRACE")]
        [Conditional("DEBUG")]
        private void CalledIfDebugOrTrace()
        {
            ConsolePrint.print("this is called on DEBUG or TRACE.");
        }

        [Conditional("DEBUG")]
        private void CalledIfTrace()
        {
            ConsolePrint.print("this is called on TRACE only.");
        }
    }

}