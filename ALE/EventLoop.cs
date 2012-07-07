using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ALE
{
    public class EventLoop
    {
        protected static ConcurrentQueue<Action> TaskQueue = new ConcurrentQueue<Action>();
        protected readonly static List<EventLoopWorker> Workers = new List<EventLoopWorker>();
        public static bool IsRunning { get; private set; }
        public static int PendingEventCount
        {
            get { return TaskQueue.Count; }
        }

        public static int IdleWorkerCount
        {
            get { return Workers.Count(x => x.Idle); }
        }

        public static int WorkerCount
        {
            get { return Workers.Count; }
        }

        public static void Start(Action startEvent)
        {
            if (startEvent != null)
            {
                Pend(startEvent);
            }
        }

        public static void Start()
        {
            IsRunning = true;
            if (Workers.Count == 0)
            {
                AddWorker();
            } else
            {
                Workers.ForEach(x => x.Start());
            }
        }

        public static void RemoveWorker()
        {
            lock (Workers)
            {
                var worker = Workers.Last();
                worker.Stop();
                worker.Wait();
                Workers.Remove(worker);
            }
        }

        public static void AddWorker()
        {
            lock (Workers)
            {
                var worker = new EventLoopWorker();
                if (IsRunning)
                {
                    worker.Start();
                }
                Workers.Add(worker);
            }
        }

        public static Action NextEvent()
        {
            Action evt;
            if (TaskQueue.TryDequeue(out evt))
            {
                return evt;
            }
            return null;
        }

        public static void Pend(Action evt)
        {
            TaskQueue.Enqueue(evt);
            Start();
        }

        public static void Stop()
        {
            Workers.ForEach(x => x.Stop());
            Workers.ForEach(x => x.Wait());
            IsRunning = false;
        }

        /// <summary>
        /// Clears all queued events.
        /// </summary>
        public static void Clear()
        {
            TaskQueue = new ConcurrentQueue<Action>();
        }
    }
}
