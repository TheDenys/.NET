using NUnit.Framework;
using Replacer.Instruments;

namespace ReplacerTests
{
    [TestFixture]
    public class TestReplace
    {

        private const string content =
@"this is a ""simple"" test
here is ""multi
 line"" entry
try Complex Prefix some text Complex Suffix
[[*]]
Guid=""*""
end
";

        private const string singleResult =
@"this is a """" test
here is """" entry
try Complex Prefix some text Complex Suffix
[[*]]
Guid=""""
end
";

        [Test]
        public void TestReplaceSingleChar()
        {
            Replacer.Replacer r = new Replacer.Replacer("\"", "\"", new EmptyStringInstrument());
            string result = r.DoReplaceInternal(content, false).ToString();
            Assert.AreEqual(singleResult, result);
        }

        [Test]
        public void TestReplaceSingleCharGarble()
        {
            Replacer.Replacer r = new Replacer.Replacer("\"", "\"", new GarbleInstrument());
            string content = @"""text""";
            string result = r.DoReplaceInternal(content, false).ToString();
            Assert.AreEqual("\"創ʇxǝʇ末\"", result);
        }

        [Test]
        public void TestReplaceMultiChar()
        {
            Replacer.Replacer r = new Replacer.Replacer("Guid=\"*", "\"", new EmptyStringInstrument());
            string content = "Guid=\"*\"";
            string result = r.DoReplaceInternal(content, false).ToString();
            Assert.AreEqual(content, result);
        }

        [Test]
        public void TestReplaceMultiCharMockGuid()
        {
            Replacer.Replacer r = new Replacer.Replacer("Guid=\"", "\"", new MockGuidInstrument());
            string content = "Guid=\"*\"";
            string result = r.DoReplaceInternal(content, false).ToString();
            Assert.AreEqual("Guid=\"278DC036-0CF8-4A71-929B-6BA0CBA1C41F\"", result);
        }

        private class EmptyStringInstrument : Instrument
        {
            public string Instrument(string original)
            {
                return string.Empty;
            }
        }

        private class MockGuidInstrument : Instrument
        {
            public string Instrument(string original)
            {
                return "278DC036-0CF8-4A71-929B-6BA0CBA1C41F";
            }
        }
    }

}
