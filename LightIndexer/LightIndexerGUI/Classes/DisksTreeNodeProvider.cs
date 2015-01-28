using System.IO;
using System.Linq;
using System.Windows.Forms;
using PDNUtils.Help;
using PDNUtils.Tree;

namespace LightIndexerGUI.Forms
{
    public class DisksTreeNodeProvider : ITreeNodeProvider
    {
        public TreeNode[] GetChildNodes()
        {
            var driveNamesChecked = CheckedManager.Instance.AllChecked.Select(p => p.Substring(0, 3).ToUpper()).Distinct();
            var driveNamesInSystem = DriveInfo.GetDrives().Select(d => d.Name);
            var driveNamesVirtual = driveNamesChecked.Where(dn => !driveNamesInSystem.Contains(dn));
            var mergedDrives = driveNamesInSystem.Union(driveNamesChecked).Distinct().Select(dn => new DriveInfo(dn));
            var driveNodes = from drive in mergedDrives
                             select new TreeNode
                                        {
                                            //NodeFont = driveNamesVirtual.Contains(drive.Name) ? virtualFont : null,
                                            Tag = driveNamesVirtual.Contains(drive.Name) ? "virtual" : null,
                                            Checked = CheckedManager.Instance.IsChecked(drive.RootDirectory.FullName),
                                            Text = drive.Name,
                                        };
            return driveNodes.ToArray();
        }
    }
}