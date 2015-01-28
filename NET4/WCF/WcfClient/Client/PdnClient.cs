using System;
using System.ServiceModel;
using System.Xml;
using WcfContract;

namespace WcfClient.Client
{
    public class PdnClient
    {

        public delegate void SendMessageHandler(string s);

        public event SendMessageHandler OnSendMessage;

        private void SendMessage(string s)
        {
            var sendMessageHandler = OnSendMessage;

            if (sendMessageHandler != null)
            {
                sendMessageHandler(s);
            }
        }

        DuplexChannelFactory<IPdnServiceDuplex> cfDuplex;
        private IPdnServiceDuplex svcDuplex;

        public void Start()
        {
            //Client((s) => SendMessage(s.IsAlive().ToString(CultureInfo.InvariantCulture)));
            //Client((s) => s.SendData(new PdnDataContainer()));
            //Client((s) => s.ReceiveData(new QueryContainer()));
            //Client((s) => s.Blow());
            //Client((s) => s.BlowOneWay());
            ClientDuplex(s => s.SendMeCallback());
        }

        public void Stop()
        {
            if (cfDuplex != null)
            {
                cfDuplex.Close();
                cfDuplex = null;
            }
            svcDuplex = null;
        }

        protected void Client(Action<IPdnService> action)
        {
            SendMessage("executing " + action);

            bool failed = false;
            ChannelFactory<IPdnService> cf = null;
            try
            {
                cf = GetNetTcpChannelFactory<IPdnService>();

                IPdnService svc = cf.CreateChannel();
                CallServiceMethod(svc, action);
                //ConsolePrint.print("getting data took:{0}ms", sw.ElapsedMilliseconds);

                (svc as ICommunicationObject).Close();
            }
            catch (Exception exception)
            {
                failed = true;

                if (cf != null)
                {
                    cf.Abort();
                }

                SendMessage(exception.Message);
                //log.Error(exception);
            }
            finally
            {
                if (cf != null && !failed)
                {
                    cf.Close();
                }
            }
        }

        protected void ClientDuplex(Action<IPdnServiceDuplex> action)
        {
            SendMessage("executing " + action);

            bool failed = false;

            try
            {
                if (cfDuplex == null)
                {
                    cfDuplex = GetDuplexChannelFactory();
                    svcDuplex = cfDuplex.CreateChannel();
                }
                
                CallDuplexServiceMethod(svcDuplex, action);
                //ConsolePrint.print("getting data took:{0}ms", sw.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                failed = true;
                SendMessage(exception.Message);
            }
        }

        private ChannelFactory<T> GetNetTcpChannelFactory<T>()
        {
            var cf = new ChannelFactory<T>(new NetTcpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxDepth = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                }
            },
            "net.tcp://localhost:8686");

            return cf;
        }

        private DuplexChannelFactory<IPdnServiceDuplex> GetDuplexChannelFactory()
        {
            InstanceContext context = new InstanceContext(new PdnServiceCallback(SendMessage));
            var cf = new DuplexChannelFactory<IPdnServiceDuplex>(
                context,
                new WSDualHttpBinding(),
                "http://localhost:11111/pdnserviceduplex");

            return cf;
        }

        private void CallServiceMethod(IPdnService svc, Action<IPdnService> action)
        {
            try
            {
                action(svc);
            }
            catch (FaultException<PdnFault> ex)
            {
                PdnFault fault = ex.Detail;
                if (fault != null)
                {
                    string msg = string.Format("code:{0}, message:'{1}'", fault.ErrorCode, fault.Message);
                    SendMessage(msg);
                }
                else
                {
                    SendMessage(ex.ToString());
                }
            }
            catch (FaultException ex)
            {
                SendMessage(ex.ToString());
            }
            catch (Exception ex)
            {
                SendMessage(ex.ToString());
            }
        }

        private void CallDuplexServiceMethod(IPdnServiceDuplex svc, Action<IPdnServiceDuplex> action)
        {
            try
            {
                action(svc);
            }
            catch (FaultException<PdnFault> ex)
            {
                PdnFault fault = ex.Detail;
                if (fault != null)
                {
                    string msg = string.Format("code:{0}, message:'{1}'", fault.ErrorCode, fault.Message);
                    SendMessage(msg);
                }
                else
                {
                    SendMessage(ex.ToString());
                }
            }
            catch (FaultException ex)
            {
                SendMessage(ex.ToString());
            }
            catch (Exception ex)
            {
                SendMessage(ex.ToString());
            }
        }

    }
}