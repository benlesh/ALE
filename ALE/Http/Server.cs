using System;
using System.Net;

namespace ALE.Http
{
	/// <summary>
	///   A simple asynchronous web server for ALE.
	/// </summary>
	public class Server : IServer
	{
		/// <summary>
		///   The HTTP listener that handles receiving requests.
		/// </summary>
		protected readonly HttpListener Listener;

		/// <summary>
		///   Creates a new instance of a server.
		/// </summary>
		/// <param name="callback"> The request received delegate. </param>
		private Server()
		{
			Listener = new HttpListener();
		}

		#region IServer Members

		/// <summary>
		///   Starts the server listening an any number of URI prefixes.
		/// </summary>
		/// <param name="prefixes"> URI prefixes for the server to listen on. </param>
		/// <returns> The server it started. </returns>
		public IServer Listen(params string[] prefixes)
		{
			if (Process == null)
				throw new InvalidOperationException("Cannot run a server with no callback. Process event must be set.");
			foreach (var prefix in prefixes)
			{
				Listener.Prefixes.Add(prefix);
			}
			if (!Listener.IsListening)
			{
				Listener.Start();
				Listener.BeginGetContext(GetContextCallback, Listener);
			}
			return this;
		}

		/// <summary>
		///   Stop the server.
		/// </summary>
		/// <param name="stopEventLoop"> </param>
		public void Stop(bool stopEventLoop = false)
		{
			Listener.Stop();
			if (stopEventLoop)
			{
				EventLoop.Stop();
			}
		}

		/// <summary>
		///   Adds preprocessing middleware.
		/// </summary>
		/// <param name="middleware"> The middleware to add. </param>
		/// <returns> The server instance. </returns>
		public IServer Use(Action<IRequest, IResponse> processor)
		{
			Process += processor;
			return this;
		}

		#endregion

		public event Action<IRequest, IResponse> Process;

		protected void OnProcess(IRequest req, IResponse res)
		{
			if (Process != null)
			{
				foreach (var invocation in Process.GetInvocationList())
				{
					if (res.Context.IsExecutionComplete) break;
					invocation.DynamicInvoke(req, res);
				}
			}
		}

		/// <summary>
		///   Creates a new server.
		/// </summary>
		/// <param name="processor"> The callback that is made when a request is received by the server. </param>
		/// <returns> An instance of a server. </returns>
		public static IServer Create()
		{
			return new Server();
		}

		/// <summary>
		///   Async callback for GetContextCallBack in Listen method above.
		/// </summary>
		/// <param name="result"> The IAsyncResult. </param>
		private void GetContextCallback(IAsyncResult result)
		{
			var listener = (HttpListener)result.AsyncState;
			var resultContext = listener.EndGetContext(result);

			var context = new ListenerContext(resultContext);
			EventLoop.Pend((t) =>
									{
										var req = context.Request;
										var res = context.Response;
										OnProcess(req, res);
									});
			listener.BeginGetContext(GetContextCallback, listener);
		}
	}
}