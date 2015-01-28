using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NET4.Parallel
{
    [TestFixture]
    public class DBWatcher_Workflow
    {
        [Test]
        public void SingleConsumer_Connected()
        {
            bool called = false;

            Func<bool> watcherRoutine =
                () =>
                {
                    called = true;
                    return true;
                };

            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx);
            var res = dbWatcher.HandleErrorAndWait(new Exception());
            Assert.True(called, "watcher routine was never called");
            Assert.True(res);
        }

        [Test]
        public void SingleConsumer_Disconnected_WatcherCancelledBeforeCall()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            bool called = false;

            Func<bool> watcherRoutine =
                () =>
                {
                    called = true;
                    return false;
                };

            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx, cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            var res = dbWatcher.HandleErrorAndWait(new Exception());
            Assert.False(called, "watcher routine was called");
            Assert.False(res);
        }

        [Test]
        public void SingleConsumer_Disconnected_WatcherCancelledAfterCall()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            bool called = false;

            Func<bool> watcherRoutine =
                () =>
                {
                    called = true;
                    return false;
                };

            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx, cancellationTokenSource.Token);
            new Task(() => { while (!called) Thread.SpinWait(100); cancellationTokenSource.Cancel(); }).Start();
            var res = dbWatcher.HandleErrorAndWait(new Exception());
            Assert.True(called, "watcher routine was not called");
            Assert.False(res);
        }

        [Test]
        public void SingleConsumer_Disconnected_StopBeforeCall()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            bool called = false;

            Func<bool> watcherRoutine =
                () =>
                {
                    called = true;
                    return false;
                };

            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx, cancellationTokenSource.Token);
            dbWatcher.Stop();
            var res = dbWatcher.HandleErrorAndWait(new Exception());
            Assert.False(called, "watcher routine was called");
            Assert.False(res);
        }

        [Test]
        public void SingleConsumer_Disconnected_StopAfterCall()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            bool called = false;

            Func<bool> watcherRoutine =
                () =>
                {
                    called = true;
                    return false;
                };

            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx, cancellationTokenSource.Token);
            new Task(() => { while (!called) Thread.SpinWait(100); dbWatcher.Stop(); }).Start();
            var res = dbWatcher.HandleErrorAndWait(new Exception());
            Assert.True(called, "watcher routine was not called");
            Assert.False(res);
        }

        [Test]
        public void SingleConsumer_Connected_AfterCall()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            bool called = false;
            bool connected = false;

            Func<bool> watcherRoutine =
                () =>
                {
                    called = true;
                    return connected;
                };

            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx, cancellationTokenSource.Token);
            new Task(() => { while (!called) Thread.SpinWait(100); connected = true; }).Start();
            var res = dbWatcher.HandleErrorAndWait(new Exception());
            Assert.True(called, "watcher routine was not called");
            Assert.True(res);
        }

    }
}