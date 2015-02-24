using System;
using System.Windows.Forms;

namespace UnitySample
{
    static class Program
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log.Info("GUI started");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form0());
            Application.ThreadExit += new EventHandler(Application_ThreadExit);
        }

        static void Application_ThreadExit(object sender, EventArgs e)
        {
            log.Info("GUI finished");
        }
    }
}
