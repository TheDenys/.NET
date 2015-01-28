using LightIndexerGUI.Classes.Views;

namespace LightIndexerGUI.Classes.Presenters
{
    public class WaitPresenter
    {
        private IWaitView waitView;

        public WaitPresenter(IWaitView waitView)
        {
            this.waitView = waitView;
        }

        public void HandleClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (waitView.Visible)
            {
                e.Cancel = true;
            }
        }
    }
}