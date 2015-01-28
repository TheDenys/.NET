using WcfContract;

namespace WcfServer.Server
{
    [ErrorBehavior(typeof(PdnErrorHandler))]
    public class PdnService2 : PdnService, IPdnService2
    {
    }
}