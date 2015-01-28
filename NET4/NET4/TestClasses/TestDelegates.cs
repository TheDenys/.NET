using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{

    [RunableClass]
    public class TestDelegates
    {

        private delegate void MyDelegate(out int i, out string s);

        private delegate void MyGenericDelegate<T1, T2>(out T1 i, out T2 s);

        [Run(0)]
        protected void Go()
        {
            MyDelegate myDelegate = (out int i, out string s) =>
            {
                i = 1;
                s = "raw";
            };

            MyGenericDelegate<int, string> myGenericDelegate = (out int i, out string s) =>
                                                    {
                                                        i = 2;
                                                        s = "generic";
                                                    };
            DoSomething(myDelegate);
            DoSomethingGeneric(myGenericDelegate);
        }

        private void DoSomething(MyDelegate myDelegate)
        {
            int i;
            string s;
            myDelegate.Invoke(out i, out s);
            ConsolePrint.print("i:{0} s:'{1}'", i, s);
        }

        private void DoSomethingGeneric(MyGenericDelegate<int, string> myGenericDelegate)
        {
            int i;
            string s;
            myGenericDelegate.Invoke(out i, out s);
            ConsolePrint.print("i:{0} s:'{1}'", i, s);
        }
    }
}