using System.IO;
using System.ServiceModel;
using WcfContract;

namespace WcfServer.Server
{

    [ServiceBehavior(
        IncludeExceptionDetailInFaults = false,
        InstanceContextMode = InstanceContextMode.Single
    )]
    public class StreamService : IPdnStreamService
    {
        public Stream GetStream()
        {
            var ms = new MemoryStream(new byte[] { 0x0a, 0x0b, 0x0c, 0x0d, 0x0e });
            return ms;
        }
    }
}