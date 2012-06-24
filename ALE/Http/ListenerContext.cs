using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Principal;
using System.Dynamic;

namespace ALE.Http
{
    public class ListenerContext : IContext
    {
        protected readonly HttpListenerContext InnerContext;
        
        internal ListenerContext(HttpListenerContext context)
        {
            InnerContext = context;
            Request = new ListenerRequest(context.Request, this);
            Response = new ListenerResponse(context.Response, this);
            User = context.User;
        }

        private dynamic _contextBag = new ExpandoObject();

        public dynamic ContextBag
        {
            get { return _contextBag; }
            set { _contextBag = value; }
        }
        public IPrincipal User { get; private set; }
        public IRequest Request { get; private set; }
        public IResponse Response { get; private set; }
    }
}
