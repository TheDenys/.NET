using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestMessageHandler
    {
        private MessageHandler mh;

        public TestMessageHandler(MessageHandler messageHandler)
        {
            mh = messageHandler;
            messageHandler.Info("just info");
        }

        [Run(0)]
        protected void SimpleTest()
        {
            mh.Debug("debug message");
        }
    }
}