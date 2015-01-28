using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class ImproperObjectConstruction
    {

        private A a;

        private B b;

        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        private volatile bool finish = false;

        [Run(0)]
        protected void Go()
        {
            Thread t = new Thread(() =>
                                      {
                                          do
                                          {
                                              autoEvent.WaitOne();
                                              ConsolePrint.print("a='{0}' b='{1}'", a, b);
                                          } while (!finish);
                                      });
            t.Start();

            a = new A();
            autoEvent.Set();

            Thread.Sleep(1000);
            b = new B(a, autoEvent);
            autoEvent.Set();

            finish = true;
        }

        private class A
        {
            public B B { get; set; }

            public override string ToString()
            {
                return string.Format("A.B={0}", B);
            }
        }

        private class B
        {
            private readonly string field1;
            private readonly string field2 = "initialized f2";

            public B(A a, AutoResetEvent autoEvent)
            {
                // exposing not-fully constructed object to outer scope
                a.B = this;
                autoEvent.Set();

                Thread.Sleep(1000);
                field1 = "initialized f1";
            }

            public override string ToString()
            {
                return string.Format("{{'{0}','{1}'}}", field1, field2);
            }
        }
    }
}