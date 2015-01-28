using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Xml;
using WcfContract;

namespace WcfServer.Server
{
    public class ServiceRunner : IDisposable
    {

        private volatile ServiceHost host, host2, host3;

        public void Start()
        {
            startServer();
        }

        public void Stop()
        {
            stopServer();
        }

        private volatile Action<string> _sendMessageAction;
        public void AddMessageAction(Action<string> action)
        {
            _sendMessageAction += action;
        }
        public void RemoveMessaage(Action<string> action)
        {
            _sendMessageAction -= action;
        }


        private void sendMessage(string s)
        {
            Action<string> send = _sendMessageAction;

            if (send != null)
            {
                send(s);
            }
        }

        private void stopServer()
        {
            try { }
            finally
            {
                try
                {
                    // host 1
                    var _host = host;

                    if (_host != null)
                    {
                        sendMessage("closing 1...");
                        _host.Close();
                        sendMessage("server 1 stopped.");
                    }

                    // host 2
                    var _host2 = host2;

                    if (_host2 != null)
                    {
                        sendMessage("closing 2...");
                        _host2.Close();
                        sendMessage("server 2 stopped.");
                    }
                    
                    // host 3
                    var _host3 = host3;

                    if (_host3 != null)
                    {
                        sendMessage("closing 3...");
                        _host3.Close();
                        sendMessage("server 3 stopped.");
                    }
                }
                catch (Exception e)
                {
                    sendMessage(e.ToString());
                    Console.WriteLine(e);
                }
            }
        }

        private void startServer()
        {
            //service 1
            Thread t = new Thread(() =>
            {
                host = null;

                try
                {
                    sendMessage("starting server 1...");

                    host = new ServiceHost(typeof(PdnService), new Uri[] { });

                    // TODO check this out
                    //host.Description.Behaviors.Add(new PdnErrorHandler());

                    ServiceEndpoint endpoint = host.AddServiceEndpoint(
                        typeof(IPdnService),
                        new NetTcpBinding
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

                    //Error handling
                    endpoint.Behaviors.Add(new PdnErrorHandler());

                    //TODO check if it'sssible to apply for one endpoint only
                    host.Description.Behaviors.Add(new ServiceMetadataBehavior()
                                                       {
                                                           HttpGetEnabled = true,
                                                           HttpGetUrl = new Uri("http://localhost:8787/pdnservice"),
                                                       });

                    ServiceEndpoint metadataExchangeEndpoint = host.AddServiceEndpoint(
                        typeof(IPdnService),
                        new BasicHttpBinding(),
                        "http://localhost:8787/pdnservice"
                        );

                    host.Open();
                    sendMessage("server 1 started");
                }
                catch (Exception exception)
                {
                    sendMessage("server 1 failed.");
                    sendMessage(exception.Message);
                    //log.Error(exception);
                }
            });

            t.IsBackground = true;
            t.Start();

            // service 2
            Thread t2 = new Thread(() =>
            {
                host2 = null;

                try
                {
                    sendMessage("starting server 2...");

                    host2 = new ServiceHost(typeof(PdnService2), new Uri[] { });

                    // TODO check this out
                    //host.Description.Behaviors.Add(new PdnErrorHandler());

                    ServiceEndpoint endpoint = host2.AddServiceEndpoint(
                        typeof(IPdnService2),
                        new NetTcpBinding
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
                        "net.tcp://localhost:9696");

                    //Error handling defined in attribute
                    //endpoint.Behaviors.Add(new PdnErrorHandler());

                    //TODO check if it's possible to apply for one endpoint only
                    host2.Description.Behaviors.Add(new ServiceMetadataBehavior()
                    {
                        HttpGetEnabled = true,
                        HttpGetUrl = new Uri("http://localhost:9797/pdnservice2"),
                    });

                    ServiceEndpoint metadataExchangeEndpoint = host2.AddServiceEndpoint(
                        typeof(IPdnService2),
                        new BasicHttpBinding(),
                        "http://localhost:9797/pdnservice2"
                        );

                    host2.Open();
                    sendMessage("server 2 started");
                }
                catch (Exception exception)
                {
                    sendMessage("server 2 failed.");
                    sendMessage(exception.Message);
                    //log.Error(exception);
                }
            });

            t2.IsBackground = true;
            t2.Start();

            // service 3
            Thread t3 = new Thread(() =>
            {
                host3 = null;

                try
                {
                    sendMessage("starting server 3...");

                    host3 = new ServiceHost(typeof(PdnServiceDuplex), new Uri[] { });

                    host3.Description.Behaviors.Add(new ServiceMetadataBehavior()
                    {
                        HttpGetEnabled = true,
                        HttpGetUrl = new Uri("http://localhost:11111/pdnserviceduplex"),
                    });

                    ServiceEndpoint endpoint = host3.AddServiceEndpoint(
                        typeof(IPdnServiceDuplex),
                        new WSDualHttpBinding(),
                        "http://localhost:11111/pdnserviceduplex");

                    host3.Open();
                    sendMessage("server 3 started");
                }
                catch (Exception exception)
                {
                    sendMessage("server 3 failed.");
                    sendMessage(exception.Message);
                    //log.Error(exception);
                }
            });

            t3.IsBackground = true;
            t3.Start();
        
        }

        public void Close()
        {
            stopServer();
            host = null;
            host2 = null;
            host3 = null;
        }

        public void Dispose()
        {
            Close();
        }
    }
}