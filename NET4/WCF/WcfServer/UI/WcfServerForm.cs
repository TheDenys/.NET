using System;
using WCFBasicUI;
using WcfServer.Server;

namespace WcfServer.UI
{
    public partial class WcfServerForm : FormBase
    {

        private ServiceRunner runner = new ServiceRunner();

        public WcfServerForm()
        {
            base.Text = "Server"; 
            base.buttonStart.Click += btnStart_Click;
            base.buttonStop.Click += btnStop_Click;
            runner.AddMessageAction(SendMessage);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            runner.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            runner.Stop();
        }
    }
}
