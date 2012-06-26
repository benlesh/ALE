using System;
using System.Dynamic;
using System.Security.Principal;
using System.Web;
using ALE.Http;

namespace ALE.Web
{
	public class AleContext : IContext
	{
		public readonly HttpContext InnerContext;
		private ExpandoObject _contextBag = new ExpandoObject();

		public AleContext(HttpContext innerContext)
		{
			if (innerContext == null) throw new ArgumentNullException("innerContext");
			InnerContext = innerContext;
			Response = new AleResponse(innerContext.Response, this);
			Request = new AleRequest(innerContext.Request, this);
		}

		#region IContext Members

		public dynamic ContextBag
		{
			get { return _contextBag; }
			set { _contextBag = value; }
		}

		public IResponse Response { get; private set; }

		public IRequest Request { get; private set; }

		public IPrincipal User { get; private set; }

		public bool IsExecutionComplete { get; private set; }

		public void Complete()
		{
			Response.Send();
			IsExecutionComplete = true;
		}

		#endregion
	}
}