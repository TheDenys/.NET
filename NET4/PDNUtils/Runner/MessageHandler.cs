using System;

namespace PDNUtils.Runner
{
    /// <summary>
    /// Class containing methods for exposing messages
    /// </summary>
    public class MessageHandler
    {

        private readonly Action<string> error;

        private readonly Action<string> info;

        private readonly Action<string> debug;

        private readonly Action<string> warn;

        /// <summary>
        /// Ctor. Also use <see cref="MessageHandlerBuilder"/> for getting instance of this class.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="info"></param>
        /// <param name="debug"></param>
        /// <param name="warn"></param>
        public MessageHandler(Action<string> error, Action<string> info, Action<string> debug, Action<string> warn)
        {
            this.error = error;
            this.info = info;
            this.debug = debug;
            this.warn = warn;
        }

        public void Error(string s) { error(s); }
        public void Info(string s) { info(s); }
        public void Debug(string s) { debug(s); }
        public void Warn(string s) { warn(s); }
    }
}