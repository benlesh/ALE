using System.Dynamic;
using System.Net;
using System.Security.Principal;

namespace ALE.Http
{
	public class ListenerContext : IContext
	{
		protected readonly HttpListenerContext InnerContext;

		private dynamic _contextBag = new ExpandoObject();

		internal ListenerContext(HttpListenerContext context)
		{
			InnerContext = context;
			Request = new ListenerRequest(context.Request, this);
			Response = new ListenerResponse(context.Response, this);
			User = context.User;
		}

		#region IContext Members

		public dynamic ContextBag
		{
			get { return _contextBag; }
			set { _contextBag = value; }
		}

		public IPrincipal User { get; private set; }
		public IRequest Request { get; private set; }
		public IResponse Response { get; private set; }

		public bool IsExecutionComplete { get; private set; }

		public void Complete()
		{
			IsExecutionComplete = true;
		}

		#endregion
	}
}