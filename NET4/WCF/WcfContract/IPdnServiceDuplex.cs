using System.ServiceModel;

namespace WcfContract
{
    [ServiceContract(Namespace = "http://www.pdn.com/wcftests/2012/duplex", CallbackContract = typeof(IPdnServiceCallback))]
    public interface IPdnServiceDuplex
    {
        [OperationContract(Name = "SendMeCallback", IsOneWay = true)]
        void SendMeCallback();
    }
}