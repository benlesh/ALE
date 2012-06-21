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
		protected static readonly ManualResetEvent StopWorkers = new ManualResetEvent(false);
		protected static readonly ManualResetEvent PauseWorkers = new ManualResetEvent(true);
		protected static readonly TaskFactory WorkerFactory = new TaskFactory();
		protected static readonly List<Task> Workers = new List<Task>();
		private static bool _started = false;
		private static EventLoop _currentEventLoop;

		public static EventLoop Current
		{
			get { return _currentEventLoop ?? (_currentEventLoop = new EventLoop()); }
		}

		public static void Start(Action begin)
		{
			Current.StartEventLoop(begin);
		}

		EventLoop()
		{

		}

		public bool IsRunning
		{
			get
			{
				return Workers.Any() && !StopWorkers.WaitOne(0);
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

		void StartEventLoop(Action begin)
		{
			if (_started)
			{
				throw new InvalidOperationException("EventLoop is already started.");
			}
			Pend(begin);
			for (int i = 0; i < WorkerCount; i++)
			{
				Workers.Add(WorkerFactory.StartNew(Worker));
			}
		}

		public static void Stop()
		{
			Current.StopEventLoop();
		}

		public void StopEventLoop()
		{
			//Stop the event loop.
			StopWorkers.Set();
			foreach (var worker in Workers)
			{
				worker.Wait();
			}
			//Clear the events.
			while (Events.Count > 0)
			{
				Action result;
				Events.TryDequeue(out result);
			}
		}
	}
}
