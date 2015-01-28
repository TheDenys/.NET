using System;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfContract;

namespace WcfClient.Client
{
    //[CallbackBehavior(UseSynchronizationContext = false)]
    public class PdnServiceCallback : IPdnServiceCallback
    {
        private readonly Action<string> report;

        public PdnServiceCallback(Action<string> report)
        {
            if (report == null) throw new ArgumentNullException("report");
            this.report = report;
        }

        public void SendStatusCallback(string status)
        {
            //Task.Factory.StartNew(() => report(status));
            report(status);
        }
    }
}