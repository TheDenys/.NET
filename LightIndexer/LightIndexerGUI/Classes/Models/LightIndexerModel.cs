using System.Collections.Generic;
using System.Collections.ObjectModel;
using LightIndexer.Indexing;
using PDNUtils.Help;

namespace LightIndexerGUI.Classes.Models
{
    internal class LightIndexerModel
    {
        public enum FormState
        {
            Undefined = 0,
            Ready = 1,
            Busy = 2,
            Closing = 3,
        }

        public SearchOptions BuildSearchOptions(
            string path,
            string content,
            bool matchWholeWord,
            int slop,
            bool wildcard
            )
        {
            var searchOptions = new SearchOptions
            {
                SearchPath = path,
                SearchString = content,
                MatchWholeWord = matchWholeWord,
                Slop = slop,
                Wildcard = wildcard,
            };

            return searchOptions;
        }

        private readonly ISet<int> markedItems = new HashSet<int>();

        public void MarkItem(int id)
        {
            markedItems.Add(id);
        }

        public void MarkItems(IEnumerable<int> ids)
        {
            ids.ForEach(id => markedItems.Add(id), false);
        }

        public void UnmarkItem(int id)
        {
            markedItems.Remove(id);
        }

        public void UnmarkItems(IEnumerable<int> ids)
        {
            ids.ForEach(id => markedItems.Remove(id), false);
        }

        public void InvalidateMark()
        {
            markedItems.Clear();
        }

        public bool IsMarked(int id)
        {
            return markedItems.Contains(id);
        }

        public IEnumerable<int> GetMarkedIds()
        {
            return new HashSet<int>(markedItems);
        } 
    }
}