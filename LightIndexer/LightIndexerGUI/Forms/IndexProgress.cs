using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LightIndexer.Config;
using LightIndexer.Indexing;

namespace LightIndexerGUI.Forms
{
    public partial class IndexProgress : Form
    {
        //logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BackgroundWorker bw;

        private Indexer indexer;

        private object sync = new object();

        private bool paused;

        public IndexProgress(IEnumerable<string> paths)
        {
            InitializeComponent();

            lblProgress.BackColor = lblProgress.Parent.BackColor;

            // inittialize Indexer class
            indexer = new Indexer(paths, Configurator.ExcludeMatcher);
            indexer.RefreshEvent += dipProgressChanged;

            //initialize BackgroundWorker
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            indexer.Start();
            indexer.WaitFor();
            lock (sync)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
            IndexerResult indexerResult = indexer.Result;

            if (indexerResult.Exception != null)
            {
                throw new ApplicationException(indexerResult.Message, indexerResult.Exception);
            }
            else
            {
                e.Result = indexerResult;
            }
        }

        private void dipProgressChanged(ProgressInfo pi)
        {
            int percent = pi.CountTotal == 0 ? 100 : pi.CountScanned <= pi.CountTotal ? (int)(100 * pi.CountScanned / pi.CountTotal) : 100;
            bw.ReportProgress(percent);
        }

        private void bw_RunWorkerCompleted(object sender,
          RunWorkerCompletedEventArgs e)
        {
            Exception exception = e.Error;
            if (e.Cancelled)
            {
                MessageBox.Show("Indexing was interrupted by user.", "Interrupted.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (exception != null)
            {
                string innerMessage = exception.InnerException.Message;
                string errMsg = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, innerMessage);
                MessageBox.Show(errMsg, "Indexing failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Indexing finished successfully!");
            }

            Close();
        }

        private void bw_ProgressChanged(object sender,
        ProgressChangedEventArgs e)
        {
            progressBarScan.Value = e.ProgressPercentage;
            lblProgress.Text = e.ProgressPercentage + " %";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var userChoice = MessageBox.Show("Dude, think twice! Indeed cancel?", "The end is close!",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (userChoice == DialogResult.No)
            {
                return;
            }

            indexer.Stop();
            bw.CancelAsync();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (paused)
            {
                indexer.Resume();
            }
            else
            {
                indexer.Pause();
            }

            paused = !paused;
            btnPause.Text = paused ? "Resume" : "Pause";
        }

        private void IndexProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (indexer.IsRunning || bw.IsBusy)
                e.Cancel = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            indexer.Dispose();
        }
    }
}