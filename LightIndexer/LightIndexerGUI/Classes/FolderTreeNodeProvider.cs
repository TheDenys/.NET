using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using log4net;
using PDNUtils.Help;
using PDNUtils.Tree;

namespace LightIndexerGUI.Forms
{
    public class FolderTreeNodeProvider : ITreeNodeProvider
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DirectoryInfo _di;

        public FolderTreeNodeProvider(string path)
        {
            _di = new DirectoryInfo(path);
        }

        public TreeNode[] GetChildNodes()
        {
            try
            {
                var startPath = _di.FullName;
                var directoryNamesChecked = from dn in CheckedManager.Instance.AllChecked
                                            where dn.ToLowerInvariant().StartsWith(startPath.ToLowerInvariant()) && string.Compare(dn, startPath, true, CultureInfo.InvariantCulture) != 0
                                            select FSHelper.FetchFolderName(startPath, dn);
                var directoryNamesInSystem = _di.Exists ? _di.GetDirectories().Select(d => d.FullName.ToLowerInvariant()) : new string[] { };

                var directoryNamesVirtual = from dn in directoryNamesChecked
                                            let ell = (from dnReal in directoryNamesInSystem select dnReal.ToLowerInvariant())
                                            where !ell.Contains(dn.ToLowerInvariant())
                                            select dn;

                var directoryInfos = directoryNamesInSystem.Union(directoryNamesVirtual).Distinct().Select(dn => new DirectoryInfo(dn));
                var folderNodes = from d in directoryInfos
                                  select new TreeNode
                                             {
                                                 Tag = d.Exists ? null : "virtual",
                                                 Text = d.Name,
                                                 Checked = CheckedManager.Instance.IsChecked(d.FullName),
                                             };
                
                return folderNodes.ToArray();
            }
            catch (Exception e)
            {
                log.Error(string.Format("GetChildNodes failed for [{0}]", _di), e);
            }

            return null;
        }
    }
}