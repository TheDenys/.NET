using System.Collections.Generic;
using System.Linq;
using LightIndexerGUI.Classes;

namespace PDNUtils.Help
{
    public class CheckedManager
    {
        private static readonly object _locker = new object();
        private static CheckedManager _instance;

        private readonly List<string> _checked;

        protected CheckedManager()
        {
            _checked = new List<string>(ProgramStateInfo.CurrentState.Indexed);
        }

        public static CheckedManager Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new CheckedManager();
                    }
                }

                return _instance;
            }
        }

        public bool IsChecked(string path)
        {
            return _checked.Contains(FSHelper.FixBackSlashes(path.ToLowerInvariant()));
        }

        public void SetChecked(string path, bool check)
        {
            lock (_locker)
            {
                var lowerInvariant = FSHelper.FixBackSlashes(path.ToLowerInvariant());
                if (check)
                { _checked.Add(lowerInvariant); }
                else
                { _checked.Remove(lowerInvariant); }
            }
        }

        public IEnumerable<string> FreshlyAdded
        {
            get
            {
                return GetFreshlyAdded();
            }
        }

        public ICollection<string> AllChecked
        {
            get
            {
                return new List<string>(ProgramStateInfo.CurrentState.Indexed);
            }
        }

        protected IEnumerable<string> GetFreshlyAdded()
        {
            var newpaths = _checked.Where(p => !ProgramStateInfo.CurrentState.Indexed.Contains(p));
            return new List<string>(newpaths);
        }

        public void PersistFresh()
        {
            ProgramStateInfo.CurrentState.AddIndexed(GetFreshlyAdded());
        }

        public void Persist()
        {
            ProgramStateInfo.CurrentState.SetIndexed(_checked);
        }
    }
}