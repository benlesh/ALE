using System;
using System.Threading;
using System.Web;

namespace ALE.Web
{
	internal class AleHttpHandlerAsync : IAsyncResult
	{
		private readonly AsyncCallback _callback;
		private readonly bool _completed;
		private readonly HttpContext _context;
		private readonly object _extraData;

		public AleHttpHandlerAsync(HttpContext context, AsyncCallback callback, object extraData)
		{
			_context = context;
			_callback = callback;
			_extraData = extraData;
			_completed = false;
		}
		
		public object AsyncState
		{
			get { return _extraData; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return null; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public void StartWork()
		{
			ThreadPool.QueueUserWorkItem(DoWork);
		}

		private void DoWork(object workItemState)
		{
			var aleContext = new AleContext(_context);
			var req = aleContext.Request;
			var res = aleContext.Response;
			EventLoop.Pend(t =>
			                       	{
			                       		Server.Create().Execute(req, res);
			                       		if (_callback != null)
			                       		{
			                       			_callback(this);
			                       		}
			                       	});
		}
	}
}