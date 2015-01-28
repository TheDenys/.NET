using log4net;

namespace NET3Console
{
    class Program
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            log.Debug("started");
        }


    }
}
