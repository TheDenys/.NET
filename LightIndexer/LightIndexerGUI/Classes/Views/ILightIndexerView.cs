using System.Diagnostics;
using LightIndexerGUI.Classes.Models;
using LightIndexerGUI.Classes.Presenters;

namespace LightIndexerGUI.Classes.Views
{
    internal interface ILightIndexerView : IActivableViewBase
    {
        LightIndexerModel.FormState CurrentFormState { get; set; }
        string PathText { get; set; }
        string SearchText { get; set; }
        bool WholeWord { get; set; }
        int Slop { get; set; }
        bool WildCard { get; set; }
        string Caption { get; set; }
        void SetStatusLabels(LightIndexerPresenter.SearchStatusContainer container);
        void ShowWait();
        void SetPathFocus();

        void HideWait();
        void ShowResults(int rowCount, Stopwatch sw);
        void UpdateFileToolstripMenu();
        void Mark(int id, bool mark);
        int GetCurrentId();
        void SetActiveCell(int id);
    }
}