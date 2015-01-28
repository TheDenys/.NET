using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LightIndexerGUI.Classes.Views
{
    public interface ITreeView
    {
        TreeNodeCollection Nodes { get; }
        Font FoldersFont { get; }
        void ShowIndexProgress(IEnumerable<string> paths);
    }
}