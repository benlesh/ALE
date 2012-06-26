using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALE.Http;
using System.Web;
using System.Dynamic;

namespace ALE.Web
{
	public class AleContext : IContext
	{
		private ExpandoObject _contextBag = new ExpandoObject();

		public readonly HttpContext InnerContext;

		public AleContext(HttpContext innerContext)
		{
			if(innerContext == null) throw new ArgumentNullException("innerContext");
			InnerContext = innerContext;
			Response = new AleResponse(innerContext.Response, this);
			Request = new AleRequest(innerContext.Request, this);
		}

		public dynamic ContextBag
		{
			get { return _contextBag; }
			set { _contextBag = value; }
		}

		public IResponse Response { get; private set; }

		public IRequest Request { get; private set; }

		public System.Security.Principal.IPrincipal User { get; private set; }

		public bool IsExecutionComplete { get; private set; }

		public void Complete()
		{
			Response.Send();
			IsExecutionComplete = true;
		}
	}
}
