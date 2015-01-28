using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Xml;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestWCF
    {
        private const int RUN = 0;

        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private System.Threading.AutoResetEvent stopFlag = new System.Threading.AutoResetEvent(false);

        [Run(RUN)]
        protected void Server()
        {
            ConsolePrint.print("running server...");

            Thread t = new Thread(() =>
            {
                ServiceHost finHost = null;

                try
                {
                    finHost = new ServiceHost(typeof(FinService), new Uri[] { });

                    finHost.AddServiceEndpoint(
                        typeof(IFinService),
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
                    finHost.Open();

                    ConsolePrint.print("listening...\npress ENTER to finish...");
                    //Console.ReadLine();
                    stopFlag.WaitOne();
                    ConsolePrint.print("closing...");
                    finHost.Close();
                    ConsolePrint.print("server stopped.");
                }
                catch (Exception exception)
                {
                    ConsolePrint.print("server failed.");
                    log.Error(exception);
                }
                finally
                {
                    if (finHost != null)
                    {
                        finHost.Close();
                    }
                }
            });

            t.Start();
            Thread.Sleep(2000);
        }

        [Run(RUN)]
        protected void Client()
        {
            ConsolePrint.print("running client...");

            bool failed = false;
            ChannelFactory<IFinService> cf = null;
            try
            {
                cf = new ChannelFactory<IFinService>(new NetTcpBinding
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
                IFinService svc = cf.CreateChannel();
                ConsolePrint.print(svc.GetCurrency("rub"));
                ConsolePrint.print(svc.GetCurrency("usd"));
                var sw = Stopwatch.StartNew();
                svc.SendData(new byte[100000000]);
                sw.Stop();
                ConsolePrint.print("sending data took:{0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                svc.GetData(100000);
                sw.Stop();
                ConsolePrint.print("getting data took:{0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                svc.GetData(1000000);
                sw.Stop();
                ConsolePrint.print("getting data took:{0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                svc.GetData(10000000);
                sw.Stop();
                ConsolePrint.print("getting data took:{0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                svc.GetData(100000000);
                sw.Stop();
                ConsolePrint.print("getting data took:{0}ms", sw.ElapsedMilliseconds);

                (svc as ICommunicationObject).Close();
            }
            catch (Exception exception)
            {
                failed = true;

                if (cf != null)
                {
                    cf.Abort();
                }

                log.Error(exception);
            }
            finally
            {
                if (cf != null && !failed)
                {
                    cf.Close();
                }
            }

            stopFlag.Set();
        }

    }

    [ServiceContract]
    public interface IFinService
    {
        [OperationContract]
        string GetCurrency(string arg);

        [OperationContract]
        void SendData(byte[] data);

        [OperationContract]
        byte[] GetData(long size);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FinService : IFinService
    {
        public string GetCurrency(string arg)
        {
            return "server:" + arg;
            //throw new NotImplementedException();
        }

        public void SendData(byte[] data)
        {
            ConsolePrint.print("received {0} bytes", data.Length);
        }

        public byte[] GetData(long size)
        {
            return new byte[size];
        }
    }
}