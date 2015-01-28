using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{

    [RunableClass]
    public class DBWatcherConsumer
    {

        private volatile bool connected;

        [Run(0)]
        public void Run()
        {
            ConsolePrint.ShowTime = true;
            CancellationTokenSource cts = new CancellationTokenSource();


            Func<bool> watcherRoutine = () =>
                                            {
                                                ConsolePrint.print("called watcher routine: {0}",
                                                                   connected ? "connected" : "disconnected");
                                                return connected;
                                            };
            Func<Exception, bool> checkEx = (e) => true;
            DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx, cts.Token);

            Action<dynamic> consumer = (bag) =>
                                           {
                                               string workerName = string.Format("{0} [{1}]", bag.Name, Task.CurrentId);
                                               int delay = bag.Delay;
                                               CancellationToken c = bag.CancellationToken;
                                               while (!c.IsCancellationRequested)
                                               {
                                                   ConsolePrint.print("consumer {0}: {1}",
                                                                      workerName,
                                                                      connected ? "connected" : "disconnected");
                                                   if (!connected)
                                                   {
                                                       var res = dbWatcher.HandleErrorAndWait(new Exception());
                                                       ConsolePrint.print("consumer {0} after wait: {1}",
                                                                          workerName,
                                                                          res ? "connected" : "disconnected");
                                                   }
                                                   if (c.IsCancellationRequested) return;
                                                   Thread.Sleep(delay);
                                               }
                                           };

            new Task(consumer, new { Name = "0", CancellationToken = cts.Token, Delay = 90 }).Start();
            new Task(consumer, new { Name = "1", CancellationToken = cts.Token, Delay = 150 }).Start();
            new Task(consumer, new { Name = "2", CancellationToken = cts.Token, Delay = 330 }).Start();
            new Task(consumer, new { Name = "3", CancellationToken = cts.Token, Delay = 450 }).Start();

            while (!cts.IsCancellationRequested)
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.R:
                        ConsolePrint.print("CONTROL: {0}", connected ? "disconnect" : "connect");
                        connected = !connected;
                        break;
                    case ConsoleKey.S:
                        dbWatcher.Stop();
                        break;
                    case ConsoleKey.Q:
                        cts.Cancel();
                        break;
                }
        }
    }

    /// <summary>
    /// This class is supposed to be used in scenarios where consumers access db with ability to retry operations.
    /// Public methods of this class are thread safe.
    /// 
    /// How to use:
    /// Step 1. Initialize DBWatcher: provide method for connection check and exceptions verification.
    /// <code>
    /// <![CDATA[
    /// Func<bool> watcherRoutine = () =>
    /// {
    ///     // check db connection
    ///     return connected;// or throw an exception
    /// };
    /// Func<Exception, bool> checkRecoverableExceptionRoutine = (e) => e is SQLException;// this method checks if it's worth retrying query operation
    /// DBWatcher dbWatcher = new DBWatcher(watcherRoutine, checkEx);
    /// ]]>
    /// </code>
    /// Step 2. Use the same instance of db watcher over several places.
    /// <code>
    /// <![CDATA[
    /// while(true)
    /// try
    /// {
    ///     //call some db operation here
    ///     break;// successfully finished operation
    /// }
    /// catch(Exception exception)
    /// {
    ///     // dbWatcher.HandleErrorAndWait(exception) inside will do the following:
    ///     // if checkRecoverableExceptionRoutine returnes true DBWatcher will make the current thread wait
    ///     // until watcherRoutine returns true
    ///     if(dbWatcher.HandleErrorAndWait(exception))
    ///     {
    ///         // it's worth retrying with such type of exception
    ///         continue;
    ///     }
    ///     else
    ///     {
    ///         // some exception type we don't want to continue with
    ///         throw;
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// Step 3. Stop or cancel DBWatcher. You can't use the same instance of DBWatcher after stop or cancel.
    /// Use <see cref="Stop"/> method to stop DBWatcher. Use <see cref="DBWatcher"/> ctor that accepts <see cref="CancellationToken"/> for cancelling.
    /// </summary>
    public class DBWatcher
    {
        /// <summary>
        /// States of DBWatcher. It's public because of unittests.
        /// </summary>
        public enum State { Waiting = 0, Ok = 1, Retrying = 2, Stopped = 3, Failed = 4 }

        private const int DEFAULT_DELAY_TIME = 1000;

        private readonly int delay;

        private readonly Func<bool> watcherRoutine;

        private readonly Func<Exception, bool> checkRecoverableExceptionRoutine;

        private readonly ManualResetEventSlim signal = new ManualResetEventSlim(true);

        private readonly object sync = new object();

        private readonly CancellationToken cancellation;

        private volatile State state;

        // defines available state transitions
        // target state <- set of source states
        private static readonly IDictionary<State, ISet<State>> availableTransitions = new Dictionary<State, ISet<State>>
        {
            {State.Stopped, new HashSet<State> {State.Stopped, State.Retrying, State.Ok, State.Waiting}},
            {State.Retrying, new HashSet<State> {State.Ok, State.Waiting}},
            {State.Ok, new HashSet<State> {State.Retrying}},
            {State.Failed, new HashSet<State> {State.Retrying}},
        };

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="watcherRoutine">Delegate used for checking the connection. Should return true when connected. Thrown exceptions are caught and db is considered disconnected.</param>
        /// <param name="checkRecoverableExceptionRoutine">Delegate for verifying exceptions. Returns true if it makes sense to retry when specified exception is caught.</param>
        /// <param name="delay">Optional. Specifies the delay between <paramref name="watcherRoutine"/> calls in milliseconds. Default value is 1 second.</param>
        public DBWatcher(Func<bool> watcherRoutine, Func<Exception, bool> checkRecoverableExceptionRoutine, int delay = DEFAULT_DELAY_TIME) :
            this(watcherRoutine, checkRecoverableExceptionRoutine, CancellationToken.None, delay) { }

        /// <summary>
        /// Instantiates cancellable <see cref="DBWatcher"/>.
        /// </summary>
        /// <param name="watcherRoutine">Delegate used for checking the connection. Should return true when connected. Thrown exceptions are caught and db is considered disconnected.</param>
        /// <param name="checkRecoverableExceptionRoutine">Delegate for verifying exceptions. Returns true if it makes sense to retry when specified exception is caught.</param>
        /// <param name="cancellation">Cancellation token. <see cref="DBWatcher"/> will not throw exceptions if token is cancelled.</param>
        /// <param name="delay">Optional. Specifies the delay between <paramref name="watcherRoutine"/> calls in milliseconds. Default value is 1 second.</param>
        public DBWatcher(Func<bool> watcherRoutine, Func<Exception, bool> checkRecoverableExceptionRoutine, CancellationToken cancellation, int delay = DEFAULT_DELAY_TIME)
        {
            this.cancellation = cancellation;
            this.cancellation.Register(signal.Set);// let consumers continue when watcher cancelled
            this.watcherRoutine = watcherRoutine;
            this.checkRecoverableExceptionRoutine = checkRecoverableExceptionRoutine;
            this.delay = delay;
        }

        /// <summary>
        /// This method calls <see cref="checkRecoverableExceptionRoutine"/> to figure out if exception is feasible for retry.
        /// If <see cref="checkRecoverableExceptionRoutine"/> returns true this method will wait until <see cref="watcherRoutine"/> returns true.
        /// </summary>
        /// <param name="exception">Exception caught by consumer of this class.</param>
        /// <returns>If exception is recoverable waits for <see cref="watcherRoutine"/> and then returns true. Returns false immediately if exception is not recoverable.</returns>
        public bool HandleErrorAndWait(Exception exception)
        {
            if (this.checkRecoverableExceptionRoutine(exception))
            {
                if (CanSwitch(this.state, State.Retrying))
                {
                    lock (this.sync)
                    {
                        if (SwitchState(State.Retrying))
                        {
                            StartWorkerAction(WatcherAction);

                            if (!this.cancellation.IsCancellationRequested)
                            {
                                signal.Reset();
                            }
                        }
                    }
                }

                signal.Wait();
                return state == State.Ok;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stops this instance. Releases all the waiting concumers.
        /// </summary>
        public void Stop()
        {
            lock (this.sync)
            {
                if (SwitchState(State.Stopped))
                {
                    signal.Set();
                }
            }
        }

        internal static bool CanSwitch(State source, State target)
        {
            ISet<State> sources;

            if (availableTransitions.TryGetValue(target, out sources))
            {
                return sources.Contains(source);
            }

            return false;
        }

        private bool SwitchState(State target)
        {
            if (CanSwitch(this.state, target))
            {
                this.state = target;
                return true;
            }

            return false;
        }

        private bool ShouldContinue()
        {
            return !this.cancellation.IsCancellationRequested && state != State.Stopped && state != State.Failed;
        }

        private void WatcherAction()
        {
            while (ShouldContinue())
            {
                try
                {
                    if (this.watcherRoutine())
                    {
                        lock (sync)
                        {
                            SwitchState(State.Ok);
                            signal.Set();
                        }
                        break;
                    }
                    else
                    {
                        Thread.Sleep(delay);
                    }
                }
                catch (Exception ex)
                {
                    if (!this.checkRecoverableExceptionRoutine(ex))
                    {
                        ConsolePrint.print("worker: Nonrecoverable");
                        lock (sync)
                        {
                            SwitchState(State.Failed);
                        }
                        break;
                    }
                }
            }

            // let consumers continue
            signal.Set();
        }

        private static void StartWorkerAction(Action action)
        {
            Task t = new Task(action);
            t.Start();
        }
    }
}