using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ALE
{
	public class EventLoop
	{
		protected static readonly ConcurrentQueue<Action> Events = new ConcurrentQueue<Action>();
		protected static readonly ManualResetEvent PauseWorkers = new ManualResetEvent(true);
		protected static readonly ManualResetEvent StopWorkers = new ManualResetEvent(false);
		protected static readonly TaskFactory WorkerFactory = new TaskFactory();
		protected static readonly List<Task> Workers = new List<Task>();
		private static bool _initialized = false;
		private static EventLoop _currentEventLoop;

		public static EventLoop Current
		{
			get { return _currentEventLoop ?? (_currentEventLoop = new EventLoop()); }
		}

		public static EventLoop Start()
		{
			return Current.StartEventLoop();
		}

		EventLoop()
		{

		}

        public bool IsRunning
        {
            get
            {
                return Workers.Any() && !PauseWorkers.WaitOne(0) && !StopWorkers.WaitOne(0);
            }
        }

		private int _workerCount = 1;

		public int WorkerCount
		{
			get { return _workerCount; }
			set { _workerCount = value; }
		}


		public Action Next()
		{
			Action evt;
			if (Events.TryDequeue(out evt))
			{
				return evt;
			}
			PauseWorkers.Reset();
			return null;
		}

		public EventLoop Pend(Action evt)
		{
			Events.Enqueue(evt);
			PauseWorkers.Set();
			return this;
		}

		protected void Worker()
		{
			while (true)
			{
				PauseWorkers.WaitOne(Timeout.Infinite);
				if (StopWorkers.WaitOne(0))
				{
					break;
				}
				var evt = Next();
				if (evt != null)
				{
					evt.Invoke();
				}
			}
		}

		EventLoop StartEventLoop()
		{
			if (_initialized)
			{
				PauseWorkers.Set();
			}
			else
			{
				for (int i = 0; i < WorkerCount; i++)
				{
					Workers.Add(WorkerFactory.StartNew(Worker));
				}
				_initialized = true;
			}
			return this;
		}

		public static EventLoop Stop()
		{
			return Current.StopEventLoop();
		}

		public EventLoop StopEventLoop()
		{
			StopWorkers.Set();
			PauseWorkers.Set();
			foreach (var worker in Workers)
			{
				worker.Wait();
			}
			return this;
		}

		public EventLoop Pause()
		{
			PauseWorkers.Reset();
			return this;
		}
	}
}
