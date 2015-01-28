namespace LightIndexerGUI.Classes.Models
{
    public interface ITreeModel
    {
        bool Visible { get; set; }
        void DeleteIndex();
    }
}