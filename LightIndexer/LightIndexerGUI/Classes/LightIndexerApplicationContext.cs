using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using LightIndexerGUI.Classes.Views;
using log4net;

namespace LightIndexerGUI.Classes
{
    internal sealed class LightIndexerApplicationContext : ApplicationContext
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly object locker = new object();

        private static IActivableViewBase lastActiveForm;

        private static readonly List<IActivableViewBase> forms = new List<IActivableViewBase>();

        internal const string ApplicationName = "Data Penetrator";

        private readonly static string APP_VERSION = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Version.ToString();

        internal static readonly string FullApplicationName = string.Format("{0} v.{1}", ApplicationName, APP_VERSION);

        internal static void SetLastForm(IActivableViewBase form)
        {
            lastActiveForm = form;
        }

        internal static bool IsLastForm
        {
            get
            {
                lock (locker)
                {
                    return forms.Count(f => f is ILightIndexerView) == 1;
                }
            }
        }

        internal static void AddForm(IActivableViewBase form)
        {
            lock (locker)
            {
                forms.Add(form);
            }
        }

        internal static void RemoveForm(IActivableViewBase form)
        {
            lock (locker)
            {
                FocusNextForm(form, false, true);
                forms.Remove(form);
            }
        }

        private static IActivableViewBase GetNextFormForActivation(IActivableViewBase current, bool next, bool remove)
        {
            lock (locker)
            {
                int count = forms.Count;

                if (count > 1)
                {
                    // finds the position right after/before active form
                    var idx = forms.FindIndex(f => f == current) + ((next) ? 1 : -1);
                    var res = idx >= 0 ? forms[idx % count] : forms.Last();

                    // check if lastActiveForm is still in forms
                    // because the form can be closed but reference will be not null yet
                    if (remove &&
                        res.IsMainForm &&
                        (lastActiveForm != null && current != lastActiveForm && forms.Contains(lastActiveForm)))
                    {
                        res = lastActiveForm;
                    }

                    log.DebugFormat("activating form: '{0}' {1}", res, current == res ? "the same" : "different");

                    return res;
                }
            }

            return null;
        }

        /// <summary>
        /// Makes active form next in list of forms.
        /// </summary>
        /// <param name="next">true - activates next form, false - activates previous</param>
        internal static void FocusNextForm(IActivableViewBase current, bool next, bool remove)
        {
            var nextForm = GetNextFormForActivation(current, next, remove);

            if (nextForm != null)
            {
                nextForm.Focus();
            }
        }

        #region overriden

        protected override void OnMainFormClosed(object sender, System.EventArgs e)
        {
            lock (locker)
            {
                var form = forms.FirstOrDefault(f => f is ILightIndexerView);

                if (form != null)
                {
                    MainForm = (Form)form;
                }
                else
                {
                    MainForm = null;
                }
            }

            if (MainForm == null)
            {
                base.OnMainFormClosed(sender, e);
            }
        }
        #endregion
    }
}