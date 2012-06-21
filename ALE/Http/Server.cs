using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ALE.Http
{
	public class Server
	{
		private readonly HttpListener _listener;
		protected readonly Action<HttpListenerRequest, Response> RequestReceived;
		public Server(Action<HttpListenerRequest, Response> callback = null)
		{
			_listener = new HttpListener();
			RequestReceived = callback;
		}

		public static Server Create(Action<HttpListenerRequest, Response> callback)
		{
			return new Server(callback);
		}
		public Server Listen(params string[] prefixes)
		{
			if (RequestReceived == null)
				throw new InvalidOperationException("Cannot run a server with no callback. RequestReceived must be set.");
			foreach (var prefix in prefixes)
			{
				_listener.Prefixes.Add(prefix);
			}
			new Task(Run).Start();
			return this;
		}

		public void Run()
		{
			_listener.Start();
			var context = _listener.BeginGetContext(GetContextCallback, _listener);
		}

		void GetContextCallback(IAsyncResult result)
		{
			var listener = (HttpListener)result.AsyncState;
			var context = listener.EndGetContext(result);
			EventLoop.Current.Pend(() => RequestReceived(context.Request, new Response(context)));
			listener.BeginGetContext(GetContextCallback, listener);
		}
		public void Stop(bool stopEventLoop = false)
		{
			_listener.Stop();
			if (stopEventLoop)
			{
				EventLoop.Stop();
			}
		}
	}
}
