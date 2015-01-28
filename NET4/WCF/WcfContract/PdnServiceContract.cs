using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfContract
{
    [ServiceContract(Namespace = "http://www.pdn.com/wcftests/v01")]
    public interface IPdnService
    {

        [OperationContract(Name = "IsAlive")]
        [FaultContract(typeof(PdnFault))]
        bool IsAlive();

        [OperationContract(Name = "SendData")]
        [FaultContract(typeof(PdnFault))]
        void SendData(PdnDataContainer container);

        [OperationContract(Name = "ReceiveData")]
        [FaultContract(typeof(PdnFault))]
        PdnDataContainer ReceiveData(QueryContainer query);

        [OperationContract(Name = "Blow", IsOneWay = false)]
        void Blow();

        [OperationContract(Name = "BlowOneWay", IsOneWay = true)]
        void BlowOneWay();

        [OperationContract(Name = "CallWithCallback")]
        void CallWithCallback();
    }

    [ServiceContract(Namespace = "http://www.pdn.com/wcftests/v02")]
    public interface IPdnService2 : IPdnService
    {
    }

    [Serializable]
    [DataContract(Name = "querydata", Namespace = "http://www.pdn.com/wcftests/v01")]
    public class QueryContainer : IExtensibleDataObject
    {
        private ExtensionDataObject extensionDataObject;
        public ExtensionDataObject ExtensionData
        {
            get { return extensionDataObject; }
            set { extensionDataObject = value; }
        }

        [DataMember(Name = "querystring")]
        private string query;

        public string Query
        {
            get { return query; }
            set { query = value; }
        }
    }

    [Serializable]
    [DataContract(Name = "PdnDataContainer", Namespace = "http://www.pdn.com/wcftests/v01")]
    public class PdnDataContainer : IExtensibleDataObject
    {
        private ExtensionDataObject extensionDataObject;
        public ExtensionDataObject ExtensionData
        {
            get { return extensionDataObject; }
            set { extensionDataObject = value; }
        }

        [DataMember(Name = "binarydata")]
        private byte[] binaryData;

        public byte[] BinaryData
        {
            get { return binaryData; }
            set { binaryData = value; }
        }

        [DataMember(Name = "stringdata")]
        private string stringData;

        public string StringData
        {
            get { return stringData; }
            set { stringData = value; }
        }
    }

    [Serializable]
    [DataContract(Name = "PdnFaultContract", Namespace = "http://www.pdn.com/wcftests/v01")]
    public class PdnFault
    {
        [DataMember(Name = "errorCode")]
        private int errorCode;

        public int ErrorCode { get { return errorCode; } set { errorCode = value; } }

        [DataMember(Name = "message")]
        private string message;

        public string Message { get { return message; } set { message = value; } }
    }
}