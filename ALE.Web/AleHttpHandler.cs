using System;
using System.Web;

namespace ALE.Web
{
	public class AleHttpHandler : IHttpAsyncHandler
	{
		#region IHttpAsyncHandler Members

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			var async = new AleHttpHandlerAsync(context, cb, extraData);
			async.StartWork();
			return async;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}

		public bool IsReusable
		{
			get { return false; }
		}


		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException();
		}

		#endregion
	}
}