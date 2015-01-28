using System;
using PDNUtils.Help;

namespace PDNUtils.Runner
{
    /// <summary>
    /// Creates <see cref="MessageHandler"/> instances.
    /// </summary>
    public class MessageHandlerBuilder
    {
        public static MessageHandler BuildConsoleMessageHandler()
        {
            return BuildSimpleMessageHandler(ConsolePrint.print);
        }

        public static MessageHandler BuildSimpleMessageHandler(Action<string> writeMessage)
        {
            return new MessageHandler(writeMessage, writeMessage, writeMessage, writeMessage);
        }


    }
}