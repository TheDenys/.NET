using System.Collections;

namespace NET4.InterviewSnippets
{

    public class YieldTest
    {


        public IEnumerable SomeMethod()
        {
            bool yield = false;
            yield return yield;
        }

    }
}