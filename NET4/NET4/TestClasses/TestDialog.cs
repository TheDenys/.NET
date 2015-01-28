using System.Diagnostics;
using System.Windows.Forms;

namespace NET4.TestClasses
{
    public partial class TestDialog : Form
    {
        public TestDialog()
        {
            InitializeComponent();
            messageRichTextBox.Text = "message with http://link";
        }

        private void TestDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void messageTichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
