using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WCFBasicUI
{
    public partial class FormBase : Form
    {
        public FormBase()
        {
            InitializeComponent();
        }

        private delegate void SendMessageCallback(string s);

        protected void SendMessage(string s)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new SendMessageCallback(SendMessage), new object[] { s });
            }
            else
            {
                textBox.AppendText(s);
                textBox.AppendText(Environment.NewLine);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox.Clear();
        }

    }
}
