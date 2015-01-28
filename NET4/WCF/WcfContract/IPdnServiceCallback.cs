using System.ServiceModel;

namespace WcfContract
{
    [ServiceContract(Namespace = "http://www.pdn.com/wcftests/2012/duplex")]
    public interface IPdnServiceCallback
    {
        [OperationContract(Name = "SendStatusCallback", IsOneWay = false)]
        void SendStatusCallback(string status);
    }
}