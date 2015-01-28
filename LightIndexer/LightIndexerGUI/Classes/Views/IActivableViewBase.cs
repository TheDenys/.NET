namespace LightIndexerGUI.Classes.Views
{
    internal interface IActivableViewBase : IView
    {
        bool IsActive { get; }
        void Focus();
        bool IsMainForm { get; }
    }
}