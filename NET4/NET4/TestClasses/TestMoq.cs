using Moq;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestMoq
    {
        [Run(0)]
        protected void SetUp()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoWork(It.IsAny<int>())).Callback((int i)=>ConsolePrint.print("call dowork"+i));
            fooMock.Setup(f => f.GetHash(It.IsAny<string>())).Returns<string>(s =>
            {
                ConsolePrint.print("arg:{0}", s);
                return s.GetHashCode();
            });

            int counter = 0;
            fooMock.Setup(f => f.GetCount()).Returns(() => counter.ToString()).Callback(() => counter++);

            IFoo foo = fooMock.Object;
            foo.DoWork(1);
            foo.DoWork(2);
            ConsolePrint.print("hash:{0}", foo.GetHash("aaa"));
            ConsolePrint.print("hash:{0}", foo.GetHash("bb"));
            ConsolePrint.print("getcount:{0}",foo.GetCount());
            ConsolePrint.print("getcount:{0}", foo.GetCount());
            ConsolePrint.print("getcount:{0}", foo.GetCount());

            foo.Bar();

            fooMock.Verify(f=>f.Bar(), Times.Once());

            //var bar = new Mock<Bar>();
            //bar.Setup(b => b.Result).Returns(28);
            //ConsolePrint.print("bar result:{0}", bar.Object.Result);
        }
    }

    public interface IFoo
    {
        void DoWork(int i);
        string GetCount();
        int GetHash(string s);
        void Bar();
    }

    public class Bar
    {
        public int Result { get { return 1; } }
    }
}