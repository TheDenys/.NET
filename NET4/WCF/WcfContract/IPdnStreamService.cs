using System.IO;
using System.ServiceModel;

namespace WcfContract
{
    [ServiceContract(Namespace = "http://www.pdn.com/wcftests/streams/v01")]
    public interface IPdnStreamService
    {
        [OperationContract(Name = "GetStream")]
        [FaultContract(typeof(PdnFault))]
        Stream GetStream();
    }
}