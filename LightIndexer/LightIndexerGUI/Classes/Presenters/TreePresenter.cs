using System.IO;
using System.Reactive.Linq;
using System.Windows.Forms;
using LightIndexer.Indexing;
using LightIndexerGUI.Classes.Models;
using LightIndexerGUI.Classes.Views;
using LightIndexerGUI.Forms;
using PDNUtils.Help;
using PDNUtils.Tree;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LightIndexerGUI.Classes.Presenters
{
    public class TreePresenter
    {
        private ITreeView treeView;
        private ITreeModel treeModel;

        public TreePresenter(ITreeView treeView, ITreeModel treeModel)
        {
            this.treeView = treeView;
            this.treeModel = treeModel;
        }

        public void OnLoad()
        {
            ITreeNodeProvider nodeProvider = new ExpandableNodeProvider(new DisksTreeNodeProvider());
            var treeNodes = nodeProvider.GetChildNodes();
            SetFont(treeNodes);
            treeView.Nodes.AddRange(treeNodes);

            //expand checked nodes
            var checkedPaths = CheckedManager.Instance.AllChecked;
            foreach (var checkedPath in checkedPaths)
            {
                ExpandPath(null, checkedPath);
            }

        }

        private void ExpandPath(TreeNode parent, string checkedPath)
        {
            var levels = checkedPath.Split(Path.DirectorySeparatorChar);
            if (levels.Length > 0 && !string.IsNullOrEmpty(levels[0]))
            {
                if (parent == null)//drives
                {
                    foreach (TreeNode node in treeView.Nodes)
                    {
                        if (node.Text.ToLowerInvariant().Contains(levels[0]))
                        {
                            if (levels.Length > 1)
                            {
                                node.Expand();
                                ExpandPath(node, checkedPath.Substring(levels[0].Length + 1));
                            }
                            else
                            {
                                node.Checked = true;
                            }
                            break;
                        }
                    }
                }
                else//folder
                {
                    foreach (TreeNode node in parent.Nodes)
                    {
                        if (node.Text.ToLowerInvariant() == levels[0])
                        {
                            if (levels.Length > 1)
                            {
                                node.Expand();
                                ExpandPath(node, checkedPath.Substring(levels[0].Length + 1));
                            }
                            else
                            {
                                node.Checked = true;
                            }
                            break;
                        }
                    }
                }
            }

        }

        public void BeforeNodeExpand(TreeNode treeNode)
        {
            var n = treeNode;
            n.Nodes.Clear();

            ITreeNodeProvider _folderTreeNodeProvider =
                new ExpandableNodeProvider(new FolderTreeNodeProvider(FSHelper.FixBackSlashes(n.FullPath)));

            var treeNodes = _folderTreeNodeProvider.GetChildNodes();

            if (treeNodes != null && treeNodes.Count() > 0)
            {
                SetFont(treeNodes);
                n.Nodes.AddRange(treeNodes);
            }
        }

        public void StartIndex()
        {
            IEnumerable<string> paths;

            CheckedManager.Instance.Persist();
            paths = CheckedManager.Instance.AllChecked;

            treeView.ShowIndexProgress(paths);
        }

        public void DeleteIndex()
        {
            treeModel.DeleteIndex();
        }

        public void AfterCheck(TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                CheckedManager.Instance.SetChecked(e.Node.FullPath, e.Node.Checked);
            }
        }

        public void SetFont(TreeNode[] treeNodes)
        {
            treeNodes.ForEachParallel(
                node =>
                {
                    if (node.Tag as string == "virtual")
                        node.NodeFont = VirtualFont();
                }
                );
        }

        private Font VirtualFont()
        {
            Font f = treeView.FoldersFont;
            return new Font(f.FontFamily, f.Size, FontStyle.Italic);
        }

    }
}