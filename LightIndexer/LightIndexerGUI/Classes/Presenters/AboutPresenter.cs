using LightIndexerGUI.Classes.Models;
using LightIndexerGUI.Classes.Views;

namespace LightIndexerGUI.Classes.Presenters
{
    public class AboutPresenter
    {
        private IAboutView view;

        private AboutModel model;

        public AboutPresenter(IAboutView aboutView)
        {
            this.view = aboutView;
            this.model = new AboutModel();
        }

        public void SetAboutInfo()
        {
            view.Description = model.Description;
        }
    }
}