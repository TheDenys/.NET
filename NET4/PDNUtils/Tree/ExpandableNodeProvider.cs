using System.Linq;
using System.Windows.Forms;

namespace PDNUtils.Tree
{
    /// <summary>
    /// Used for virtual mode of <see cref="TreeView"/>
    /// </summary>
    public class ExpandableNodeProvider : ITreeNodeProvider
    {
        private ITreeNodeProvider nodeProvider;

        public ExpandableNodeProvider(ITreeNodeProvider nodeProvider)
        {
            this.nodeProvider = nodeProvider;
        }

        public TreeNode[] GetChildNodes()
        {
            var childNodes = nodeProvider.GetChildNodes();
            if (childNodes == null)
            {
                return null;
            }
            var nodesWithDummy = from n in childNodes
                                 select new TreeNode(n.Text, new TreeNode[] { new TreeNode("dummy"), })
                                            {
                                                NodeFont = n.NodeFont,
                                                //Tag = "dummy",
                                                Tag = n.Tag,
                                                Checked = n.Checked
                                            };
            return nodesWithDummy.ToArray();
        }
    }
}