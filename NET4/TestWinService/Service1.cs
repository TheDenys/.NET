using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using log4net;

namespace TestWinService
{
    public partial class TestWinService : ServiceBase
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TestWinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            FileStream fs = new FileStream(@"d:\mcWindowsService.txt",
            FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" mcWindowsService: Service Started \n");
            m_streamWriter.Flush();
            m_streamWriter.Close();
            log.Info("vot tak vot i jiviom");
        }

        protected override void OnStop()
        {
            FileStream fs = new FileStream(@"d:\mcWindowsService.txt",
            FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" mcWindowsService: Service Stopped \n"); m_streamWriter.Flush();
            m_streamWriter.Close();
            log.Info("dveri zakrivayutsa");
        }
    }
}
