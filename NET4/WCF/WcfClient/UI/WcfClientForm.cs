using System;
using WCFBasicUI;
using WcfClient.Client;

namespace WcfClient.UI
{
    public partial class WcfClientForm : FormBase
    {
        private PdnClient client;

        public WcfClientForm()
        {
            base.Text = "Client";
            base.buttonStart.Click += buttonStart_Click;
            base.buttonStop.Click += buttonStop_Click;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            client = new PdnClient();
            client.OnSendMessage += SendMessage;
            client.Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Stop();
                client = null;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(827, 4);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 4);
            // 
            // WcfClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(603, 353);
            this.Name = "WcfClientForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
