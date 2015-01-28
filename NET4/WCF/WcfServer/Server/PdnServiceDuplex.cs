using System.ServiceModel;
using System.Threading.Tasks;
using WcfContract;

namespace WcfServer.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    //[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class PdnServiceDuplex : IPdnServiceDuplex
    {
        public void SendMeCallback()
        {
            var ch = OperationContext.Current.GetCallbackChannel<IPdnServiceCallback>();

            // call with ConcurrencyMode.Single and IsOneWay = false
            // will cause a dedlock because this is not one way operation
            // and client is awaiting for finishing this call, server in it's turn is trying to call method on client
            ch.SendStatusCallback("status!!!");
        }
    }
}