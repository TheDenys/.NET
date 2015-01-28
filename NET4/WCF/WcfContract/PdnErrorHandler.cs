using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WcfContract
{
    public class PdnErrorHandler : IErrorHandler, IEndpointBehavior
    {

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // operation trew uncaught exception so we'll provide a fault for it
            if (fault == null)
            {
                if (error is NotImplementedException)
                {
                    FaultException fe = new FaultException(error.Message);
                    MessageFault mf = fe.CreateMessageFault();
                    fault = Message.CreateMessage(version, mf, fe.Action);
                }
            }
        }

        public bool HandleError(Exception error)
        {
            if (error is NotImplementedException)
            {
                return false;
            }

            return true;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            
            if (channelDispatcher != null)
            {
                channelDispatcher.ErrorHandlers.Add(this);
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }

}