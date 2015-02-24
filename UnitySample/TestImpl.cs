namespace UnitySample
{
    
    public enum TestEnum
    {
        Zero,
        One
    }
    
    internal class TestImpl : ITest
    {

        public int Number { get; set; }

        public string Text { get; set; }

        public TestEnum TestEnum { get; set; }

        public TestImpl(int number, string text, TestEnum testEnum)
        {
            Number = number;
            Text = text;
            TestEnum = testEnum;
        }
    }
}