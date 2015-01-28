using System.Windows.Forms;
using LightIndexerGUI.Classes.Presenters;
using LightIndexerGUI.Classes.Views;
using PDNUtils.Help;

namespace LightIndexerGUI.Forms
{
    public partial class WaitForm : Form, IWaitView
    {
        private readonly WaitPresenter waitPresenter;

        public WaitForm()
        {
            InitializeComponent();
            this.waitPresenter = new WaitPresenter(this);
        }

        public void ShowView(Form form)
        {
            this.Show(form);
            this.CenterFormTo(form);
        }

        public void HideView()
        {
            this.Hide();
        }

        private void WaitForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            waitPresenter.HandleClosing(sender, e);
        }

        bool IWaitView.Visible { get { return this.Visible; } }

    }
}
