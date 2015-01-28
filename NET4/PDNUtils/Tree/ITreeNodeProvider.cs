using System.Windows.Forms;

namespace PDNUtils.Tree
{
    /// <summary>
    /// interface for <see cref="TreeNode"/> providing
    /// </summary>
    public interface ITreeNodeProvider
    {
        TreeNode[] GetChildNodes();
    }
}