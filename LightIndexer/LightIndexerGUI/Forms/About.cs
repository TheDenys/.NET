using System;
using System.Windows.Forms;
using LightIndexerGUI.Classes.Presenters;
using LightIndexerGUI.Classes.Views;

namespace LightIndexerGUI.Forms
{
    [Obsolete("Use AboutBox")]
    public partial class About : Form, IAboutView
    {

        private AboutPresenter presenter;

        public About()
        {
            presenter = new AboutPresenter(this);
            InitializeComponent();
            presenter.SetAboutInfo();
        }

        public string Description
        {
            get { return lblDescritpion.Text; }
            set { lblDescritpion.Text = value; }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
