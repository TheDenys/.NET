using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Replacer.Instruments;

namespace Replacer
{
    public class Replacer : IDisposable
    {

        private readonly BackgroundWorker bw = new BackgroundWorker();

        private readonly string filename;

        private readonly Tuple<string, string> pattern;

        private bool currentConfirm = false;

        public Action OnFinish;

        public Action<Tuple<string, string>> OnConfirm;

        private Tuple<string, string> currentConfirmData;

        private Instrument currentInstrument;

        private volatile bool cancelled;

        private volatile int percentDone;

        public Replacer(string prefix, string suffix, Instrument instrument)
        {
            if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(suffix))
            {
                throw new ArgumentException("Both prefix and suffix must have values!");
            }

            if (instrument == null)
            {
                throw new ArgumentNullException("instrument");
            }

            currentInstrument = instrument;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += (sender, args) => DoReplace();

            bw.ProgressChanged += (sender, args) =>
            {
                if (OnConfirm != null)
                {
                    OnConfirm(currentConfirmData);
                }
            };

            bw.RunWorkerCompleted += (sender, args) =>
            {
                if (OnFinish != null)
                {
                    OnFinish();
                }
            };

            pattern = Tuple.Create(prefix, suffix);
        }

        public Replacer(string filename, string prefix, string suffix, Instrument instrument)
            : this(prefix, suffix, instrument)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("filename can't be empty");
            }

            this.filename = filename;
        }

        public void Start()
        {
            bw.RunWorkerAsync();
        }

        private void DoReplace()
        {
            try
            {
                string content = File.ReadAllText(filename);
                StringBuilder result = DoReplaceInternal(content, true);
                File.WriteAllText(filename + ".rep", result.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        internal StringBuilder DoReplaceInternal(string content, bool needsConfirm)
        {
            string pref = pattern.Item1;
            string suff = pattern.Item2;

            StringBuilder result = new StringBuilder();

            int startPos = 0;
            int idx1, idx2;
            string buf = null, nakedbuf = null, instrumented = null;
            bool confirm = false;

            while (startPos < content.Length && !cancelled)
            {
                idx1 = content.IndexOf(pref, startPos, System.StringComparison.Ordinal);

                if (idx1 != -1)
                {
                    idx2 = content.IndexOf(suff, idx1 + pref.Length, System.StringComparison.Ordinal);

                    if (idx2 != -1)
                    {
                        result.Append(content.Substring(startPos, idx1 - startPos));

                        buf = content.Substring(idx1, idx2 - idx1 + 1);

                        // cut off suffix and prefix before instrumentation
                        nakedbuf = buf.Substring(pref.Length, buf.Length - (pref.Length + suff.Length));
                        instrumented = pattern.Item1 + InstrumentText(nakedbuf) + pattern.Item2;

                        startPos = idx2 + 1;
                        confirm = needsConfirm ? Confirm(buf, instrumented, (100 * startPos / content.Length)) : true;
                        result.Append(confirm ? instrumented : buf);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            result.Append(content.Substring(startPos));

            return result;
        }

        private string InstrumentText(string orig)
        {
            return currentInstrument.Instrument(orig);
        }

        private bool Confirm(string t1, string t2, int startPos)
        {
            currentConfirmData = Tuple.Create(t1, t2);
            percentDone = startPos;
            bw.ReportProgress(startPos, new object());

            //wait
            lock (this)
            {
                if (!cancelled)
                {
                    Monitor.Wait(this);
                }
            }

            return currentConfirm;
        }

        public int PercentDone { get { return percentDone; } }

        public void SendConfirm(bool yn)
        {
            currentConfirm = yn;
            //continue
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }

        private void SetCancelled()
        {
            cancelled = true;
        }

        public void Close()
        {
            SetCancelled();

            lock (this)
            {
                Monitor.PulseAll(this);
            }

            bw.Dispose();
        }

        public void Dispose()
        {
            bw.Dispose();
        }
    }
}