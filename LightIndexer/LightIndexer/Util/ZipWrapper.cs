using System;
using System.IO;
using System.Linq;
using LightIndexer.Config;
using PDNUtils.IO;

namespace LightIndexer.Util
{
    public class ZipWrapper
    {
        public static Tuple<string, Stream> GetFileFromZip(string filename)
        {
            var filesFromZip = ZipHelper.GetFileFromZip(filename, Constants.MAX_FILE_SIZE);
            return filesFromZip != null ? filesFromZip.FirstOrDefault() : null;
        }
    }
}