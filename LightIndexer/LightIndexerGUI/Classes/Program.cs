using System;
using System.Reflection;
using System.Windows.Forms;
using LightIndexerGUI.Forms;
using log4net;

namespace LightIndexerGUI.Classes
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AssemblyName assemblyName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName();
            var name = assemblyName.Name;
            var version = assemblyName.Version;
            log.InfoFormat("started GUI {0} v.{1}", name, version);

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var applicationContext = new LightIndexerApplicationContext();
            applicationContext.MainForm = new SplashScreen(applicationContext);
            Application.Run(applicationContext);

            log.Info("exited GUI");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                log.FatalFormat("IsTerminating: {0}", e.IsTerminating);
                log.Fatal("Unhandled exception.", e.ExceptionObject as Exception);
                MessageBox.Show(
                    "Application has to be closed because of unexpected error.", 
                    "Error.", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            catch (Exception)
            {
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Cancel;

            try
            {
                log.Fatal("Unhandled exception.", e.Exception);
                result = ShowThreadExceptionDialog("Windows Forms Error", e.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        // Creates the error message and displays it.
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "An application error occurred.\n\n";
            errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }
    }
}
