using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ALE.Http;
using System;
using System.Threading;

namespace ALE.Web
{
	public class AleHttpHandler : IHttpAsyncHandler
	{
		private readonly Action<HttpContext> _processorDelegate;

		public AleHttpHandler()
		{
			_processorDelegate = ProcessRequest;
		}
		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			return _processorDelegate.BeginInvoke(context, EndProcessRequest, extraData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			_processorDelegate.EndInvoke(result);
		}

		public bool IsReusable
		{
			get { return false; }
		}

		public void ProcessRequest(HttpContext context)
		{
			//var aleContext = new AleContext(context);
			//var req = aleContext.Request;
			//var res = aleContext.Response;
			context.Response.Write("Hello World");
			//EventLoop.Current.Pend(() => Server.Create().Execute(req, res));
		}
	}
}
