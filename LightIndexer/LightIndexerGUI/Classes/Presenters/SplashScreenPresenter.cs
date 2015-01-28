using LightIndexerGUI.Classes.Views;
using LightIndexerGUI.Forms;

namespace LightIndexerGUI.Classes.Presenters
{
    public class SplashScreenPresenter
    {
        
        private ISplashScreenView view;

        public SplashScreenPresenter(ISplashScreenView view)
        {
            this.view = view;
        }

        public void Init()
        {
            var f = new LightIndexerForm();
        }
    }
}