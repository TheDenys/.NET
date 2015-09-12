using System.Linq;

namespace PDNUtils.Runner
{
    public abstract class RunableBase
    {
        protected MessageHandler MessageHandler
        {
            get;
            private set;
        }

        /// <summary>
        /// Default ctor.
        /// </summary>
        protected RunableBase()
        {
        }

        /// <summary>
        /// Use this ctor. if class wants to set own message handler.
        /// </summary>
        /// <param name="mh"></param>
        protected RunableBase(MessageHandler mh)
        {
            this.MessageHandler = mh;
        }

        protected void Debug(object message)
        {
            if (MessageHandler != null)
                MessageHandler.Debug(Help.Utils.GetString(message));
        }

        protected void DebugFormat(string format, params object[] messages)
        {
            if (MessageHandler != null)
                MessageHandler.Debug(string.Format(format, messages.Select(Help.Utils.GetString).ToArray()));
        }
    }
}