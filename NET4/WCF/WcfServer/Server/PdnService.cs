using System.ServiceModel;
using WcfContract;

namespace WcfServer.Server
{
    [ServiceBehavior(
        //IncludeExceptionDetailInFaults = true,
        IncludeExceptionDetailInFaults = false,
        InstanceContextMode = InstanceContextMode.Single
    )]
    public class PdnService : IPdnService
    {
        public bool IsAlive()
        {
            return true;
        }

        public void SendData(PdnDataContainer container)
        {
            throw new System.NotImplementedException();
        }

        public PdnDataContainer ReceiveData(QueryContainer query)
        {
            throw new FaultException<PdnFault>(new PdnFault { ErrorCode = 0x17855, Message = "NotImplemented" });
        }

        public void Blow()
        {
            throw new System.NotImplementedException("Blow");
        }

        public void BlowOneWay()
        {
            throw new System.NotImplementedException("BlowOneWay");
        }

        public void CallWithCallback()
        {
            // just nothing so far
        }
    }
}