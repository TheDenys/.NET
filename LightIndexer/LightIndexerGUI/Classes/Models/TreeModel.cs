using LightIndexer.Config;

namespace LightIndexerGUI.Classes.Models
{
    public class TreeModel : ITreeModel
    {
        public bool Visible { get; set; }
        public void DeleteIndex()
        {
            Configurator.GetDefaultIndexManager().DeleteIndex();
        }
    }
}