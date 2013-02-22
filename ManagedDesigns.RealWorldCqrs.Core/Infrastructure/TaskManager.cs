using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManagedDesigns.RealWorldCqrs.Core.Infrastructure
{
    public class TaskManager
    {
        private readonly int retryTimes = 5;
        private readonly TimeSpan retryWaitTime = TimeSpan.FromSeconds(10);

        private readonly IDictionary<Guid, Task> tasks = new Dictionary<Guid, Task>();
        private bool Runned = true;

        public TaskManager(TimeSpan retryWaitTime, int retryTimes = int.MaxValue)
        {
            this.retryWaitTime = retryWaitTime;
            this.retryTimes = retryTimes;
        }

        public IEnumerable<Guid> ActiveTasks
        {
            get { return tasks.Keys.ToArray(); }
        }

        public WhenThen When(Func<bool> when)
        {
            return new WhenThen(this, when);
        }

        public void Execute(Action action)
        {
            Add(() =>
                    {
                        action();
                        return true;
                    });
        }

        internal void Add(Func<bool> action)
        {
            Guid taskId = Guid.NewGuid();
            var task = new Task((id) =>
                                    {
                                        int times = retryTimes;
                                        while (times == 0 || Runned)
                                        {
                                            bool completed = action();
                                            if (completed)
                                            {
                                                lock (tasks)
                                                {
                                                    tasks.Remove((Guid) id);
                                                    // Console.WriteLine("Removed task {0} from TaskManager. Remaining tasks {1}", id, this.tasks.Count);
                                                }
                                                return;
                                            }
                                            times--;
                                            Thread.Sleep(retryWaitTime);
                                        }
                                    }, taskId);
            lock (tasks)
            {
                //  Console.WriteLine("Added task {0} to TaskManager. Remaining tasks {1}", taskId, this.tasks.Count);
                tasks.Add(taskId, task);
            }
            task.Start();
        }

        public void Stop()
        {
            Runned = false;
            tasks.Clear();
        }

        #region Nested type: WhenThen

        public class WhenThen
        {
            private readonly TaskManager taskManager;
            private readonly Func<bool> when;

            internal WhenThen(TaskManager taskManager, Func<bool> when)
            {
                this.taskManager = taskManager;
                this.when = when;
            }

            public void Then(Action then)
            {
                taskManager.Add(() =>
                                    {
                                        if (!when())
                                        {
                                            return false;
                                        }
                                        then();
                                        return true;
                                    });
            }
        }

        #endregion
    }
}