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

		public Server Listen(string host, int port = 80, string scheme = "http")
		{
			if (!EventLoop.Current.IsRunning)
			{
				EventLoop.Start();
			}
			if (RequestReceived == null)
				throw new InvalidOperationException("Cannot run a server with no callback. RequestReceived must be set.");
			_listener.Prefixes.Add(CreatePrefix(host, port, scheme));
			new Task(Run).Start();
			return this;
		}

		public void Run()
		{
			_listener.Start();
			while (_listener.IsListening)
			{
				var context = _listener.GetContext();
				EventLoop.Current.Pend(() => RequestReceived(context.Request, new Response(context)));
			}
		}

        public void Stop(bool stopEventLoop = false)
        {
            _listener.Stop();
            if (stopEventLoop)
            {
                EventLoop.Stop();
            }
        }
		public static string CreatePrefix(string host, int port = 80, string scheme = "http")
		{
			var builder = new UriBuilder();
			builder.Host = host;
			builder.Scheme = scheme;
			builder.Port = port;
			return builder.Uri.ToString();
		}
	}
}
