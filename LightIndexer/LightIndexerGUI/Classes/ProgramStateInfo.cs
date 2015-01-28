using System;
using System.Collections.Generic;
using System.Linq;
using PDNUtils.Config;

namespace LightIndexerGUI.Classes
{
    public class ProgramStateInfo
    {
        public enum ProgramState
        {
            Idle,
            Searching,
            Indexing,
        }

        private Action update;

        public ProgramStateInfo()
        {
            Indexed = new List<string>();
            update = () => ProgramStateManager<ProgramStateInfo>.Instance.Save(this);
        }

        public List<string> Indexed { get; set; }

        public void AddIndexed(string path)
        {
            Indexed.Add(path);
            update();
        }

        public void AddIndexed(IEnumerable<string> paths)
        {
            var newpaths = paths.Where(p => !Indexed.Contains(p));
            Indexed.AddRange(newpaths);
            update();
        }

        public void SetIndexed(IEnumerable<string> paths)
        {
            Indexed.Clear();
            Indexed.AddRange(paths);
            update();
        }

        public static ProgramStateInfo CurrentState
        {
            get
            {
                return InnerCurrent();
            }
        }

        private static ProgramStateInfo InnerCurrent()
        {
            return ProgramStateManager<ProgramStateInfo>.Instance.Load();
        }
    }
}