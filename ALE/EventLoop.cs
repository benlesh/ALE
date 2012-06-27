using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace ALE
{
	public class EventLoop
	{
		public readonly static ConcurrentQueue<Action<Task>> TaskQueue = new ConcurrentQueue<Action<Task>>();
		protected static ManualResetEvent PauseWorkers = new ManualResetEvent(true);
		protected static ManualResetEvent StopWorkers = new ManualResetEvent(false);
		private static int _workerCount = 1;
		private static Task[] _workers;

		public static int WorkerCount
		{
			get { return _workerCount; }
			set
			{
				if (_workerCount <= 0) throw new ArgumentOutOfRangeException("WorkerCount must be a positive integer");
				_workerCount = value;
			}
		}


		public static void Start(Action<Task> startEvent = null)
		{
			if (startEvent != null)
			{
				Pend(startEvent);
			}
			_workers = new Task[WorkerCount];
			for (int i = 0; i < WorkerCount; i++)
			{
				_workers[i] = Task.Factory.StartNew(() =>
													{
														var t = Task.Factory.StartNew(() => { });
														while (true)
														{
															PauseWorkers.WaitOne(Timeout.Infinite);
															if (StopWorkers.WaitOne(0)) break;
															var evt = Next();
															if (evt != null)
															{
																t.ContinueWith(evt);
															}
														}
													});
			}
		}

		public static Action<Task> Next()
		{
			Action<Task> evt;
			if (TaskQueue.TryDequeue(out evt))
			{
				return evt;
			}
			PauseWorkers.Reset();
			return null;
		}

		public static void Pend(Action<Task> evt)
		{
			TaskQueue.Enqueue(evt);
			PauseWorkers.Set();
		}

		public static void Stop()
		{
			PauseWorkers.Set();
			StopWorkers.Set();
			Task.WaitAll(_workers);
		}
	}
}
