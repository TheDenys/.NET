using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace LightIndexerGUI.Classes.Help
{
    public static class StartEditor
    {
        private const string Notepad = "notepad";

        private static string editorExe;

        static StartEditor()
        {
            string appSetting = ConfigurationManager.AppSettings["EDITOR"];

            bool useDefault = string.IsNullOrWhiteSpace(appSetting) || !File.Exists(appSetting);

            if (useDefault)
            {
                editorExe = Notepad;
            }
            else
            {
                editorExe = appSetting;
            }
        }

        public static void Start(string path)
        {
            ProcessStartInfo psi = new ProcessStartInfo(editorExe, path);
            Process.Start(psi);
        }
    }
}