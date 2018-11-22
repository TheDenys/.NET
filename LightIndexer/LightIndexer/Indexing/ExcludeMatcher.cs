using DotNet.Globbing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightIndexer.Indexing
{
    public class ExcludeMatcher
    {
        private readonly List<Glob> globs;

        public ExcludeMatcher(string patternsClob)
        {
            if (string.IsNullOrEmpty(patternsClob)) return;
            var patterns = patternsClob.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            globs = new List<Glob>(patterns.Length);
            GlobOptions options = new GlobOptions { Evaluation = { CaseInsensitive = false } };

            foreach (var p in patterns)
            {
                Glob glob = Glob.Parse(p, options);
                globs.Add(glob);
            }
        }

        public bool IsExcluded(string path)
        {
            if (globs == null) return false;

            var excluded = false;

            Parallel.ForEach(globs, (glob, state) =>
            {
                if (excluded) return;

                if (glob.IsMatch(path))
                {
                    excluded = true;
                    state.Stop();
                }
            });

            return excluded;
        }
    }
}
