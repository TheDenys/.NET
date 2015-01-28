using NUnit.Framework;

namespace NET4.Parallel
{

    [TestFixture]
    public class DBWatcher_StatesTransitions
    {
        [TestCase(DBWatcher.State.Ok, DBWatcher.State.Ok, Result = false)]
        [TestCase(DBWatcher.State.Ok, DBWatcher.State.Retrying, Result = true)]
        [TestCase(DBWatcher.State.Ok, DBWatcher.State.Failed, Result = false)]
        [TestCase(DBWatcher.State.Ok, DBWatcher.State.Stopped, Result = true)]
        [TestCase(DBWatcher.State.Ok, DBWatcher.State.Waiting, Result = false)]

        [TestCase(DBWatcher.State.Retrying, DBWatcher.State.Ok, Result = true)]
        [TestCase(DBWatcher.State.Retrying, DBWatcher.State.Retrying, Result = false)]
        [TestCase(DBWatcher.State.Retrying, DBWatcher.State.Failed, Result = true)]
        [TestCase(DBWatcher.State.Retrying, DBWatcher.State.Stopped, Result = true)]
        [TestCase(DBWatcher.State.Retrying, DBWatcher.State.Waiting, Result = false)]

        [TestCase(DBWatcher.State.Failed, DBWatcher.State.Ok, Result = false)]
        [TestCase(DBWatcher.State.Failed, DBWatcher.State.Retrying, Result = false)]
        [TestCase(DBWatcher.State.Failed, DBWatcher.State.Failed, Result = false)]
        [TestCase(DBWatcher.State.Failed, DBWatcher.State.Stopped, Result = false)]
        [TestCase(DBWatcher.State.Failed, DBWatcher.State.Waiting, Result = false)]

        [TestCase(DBWatcher.State.Stopped, DBWatcher.State.Ok, Result = false)]
        [TestCase(DBWatcher.State.Stopped, DBWatcher.State.Retrying, Result = false)]
        [TestCase(DBWatcher.State.Stopped, DBWatcher.State.Failed, Result = false)]
        [TestCase(DBWatcher.State.Stopped, DBWatcher.State.Stopped, Result = true)]
        [TestCase(DBWatcher.State.Stopped, DBWatcher.State.Waiting, Result = false)]

        [TestCase(DBWatcher.State.Waiting, DBWatcher.State.Ok, Result = false)]
        [TestCase(DBWatcher.State.Waiting, DBWatcher.State.Retrying, Result = true)]
        [TestCase(DBWatcher.State.Waiting, DBWatcher.State.Failed, Result = false)]
        [TestCase(DBWatcher.State.Waiting, DBWatcher.State.Stopped, Result = true)]
        [TestCase(DBWatcher.State.Waiting, DBWatcher.State.Waiting, Result = false)]
        public bool CanSwitch(DBWatcher.State source, DBWatcher.State target)
        {
            return DBWatcher.CanSwitch(source, target);
        }
    }
}