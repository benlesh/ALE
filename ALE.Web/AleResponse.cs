using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ALE.Http;
using ALE.Views;

namespace ALE.Web
{
	public class AleResponse : IResponse
	{
		public readonly HttpResponse InnerResponse;

		public AleResponse(HttpResponse innerResponse, IContext context)
		{
			if (innerResponse == null) throw new ArgumentNullException("innerResponse");
			if (context == null) throw new ArgumentNullException("context");
			InnerResponse = innerResponse;
			Context = context;
			ContentType = "text/html";
			StatusCode = 200; //OK
		}
		
		public IContext Context { get; private set; }

		public string ContentType
		{
			get { return InnerResponse.ContentType; }
			set { InnerResponse.ContentType = value; }
		}

		public Encoding ContentEncoding
		{
			get { return InnerResponse.ContentEncoding; }
			set { InnerResponse.ContentEncoding = value; }
		}

		public CookieCollection Cookies
		{
			get { throw new NotImplementedException("Sorry"); }
		}

		public NameValueCollection Headers
		{
			get { return InnerResponse.Headers; }
		}

		public Stream OutputStream
		{
			get { return InnerResponse.OutputStream; }
		}

		public string RedirectLocation
		{
			get { return InnerResponse.RedirectLocation; }
			set { InnerResponse.RedirectLocation = value; }
		}

		public int StatusCode
		{
			get { return InnerResponse.StatusCode; }
			set { InnerResponse.StatusCode = value; }
		}

		public string StatusDescription
		{
			get { return InnerResponse.StatusDescription; }
			set { InnerResponse.StatusDescription = value; }
		}

		public IViewProcessor ViewProcessor { get; set; }

		public void AddHeader(string name, string value)
		{
			InnerResponse.AddHeader(name, value);
		}

		public void AppendCookie(Cookie cookie)
		{
			throw new NotImplementedException();
		}

		public void AppendHeader(string name, string value)
		{
			InnerResponse.AppendHeader(name, value);
		}

		public IResponse Write(string text)
		{
			InnerResponse.Write(text);
			return this;
		}

		public IResponse Write(byte[] binary)
		{
			InnerResponse.BinaryWrite(binary);
			return this;
		}

		public void Send()
		{
			InnerResponse.Flush();
			InnerResponse.Close();
		}

		public void Redirect(string location)
		{
			InnerResponse.Redirect(location);
		}

		public IResponse Render(string view, object model, Action<Exception> callback)
		{
			if (ViewProcessor == null)
			{
				throw new InvalidOperationException("ViewProcessor has not been set. Unable to render view.");
			}
			ViewProcessor.Render(view, model, (ex, rendered) =>
												  {
													  if (ex != null && callback != null)
													  {
														  EventLoop.Pend(() => callback(ex));
														  return;
													  }
													  Write(rendered);
												  });
			return this;
		}
	}
}