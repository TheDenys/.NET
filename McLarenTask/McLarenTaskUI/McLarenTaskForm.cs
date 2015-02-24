using System.Windows.Forms;
using McLarenTask;

namespace McLarenTaskUI
{
    public partial class McLarenTaskForm : Form
    {
        public McLarenTaskForm()
        {
            InitializeComponent();
        }

        private void btnFind_Click(object sender, System.EventArgs e)
        {
            PalindromeSeeker palindromeSeeker = new PalindromeSeeker(tbInput.Text);

            foreach (var palindrome in palindromeSeeker.GetFirstNLongestPalindromes((int)numUpDownTop.Value))
            {
                tbResult.AppendText(palindrome.ToString());
                tbResult.AppendText("\n");
            }

            tbResult.AppendText("\n");
        }
    }
}
