using System.Threading.Tasks;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{
    [RunableClass]
    public class AsyncAwaitTest : RunableBase
    {
        [Run(0)]
        async void DoStuff()
        {
            DebugFormat("something is happening");

            string res = await GetResultAsync();

        }

        async Task<string> GetResultAsync()
        {
            var resTask = GetInnerResult();
            Debug("something else is happenning inbetween...");
            var s = await resTask;
            return s;
        }

        async Task<string> GetInnerResult()
        {
            Task<string> innerTask = Task.Factory.StartNew(() =>
            {
                DebugFormat("calculating result...");
                return "async result";
            });
            DebugFormat("waiting for results calculating...");
            string res = await innerTask;
            return res;
        }
    }
}