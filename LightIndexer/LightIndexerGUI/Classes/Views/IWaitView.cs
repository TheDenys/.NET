using System.Windows.Forms;

namespace LightIndexerGUI.Classes.Views
{
    public interface IWaitView
    {
        void ShowView(Form form);
        void HideView();
        bool Visible { get; }
    }
}