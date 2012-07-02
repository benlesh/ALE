using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using ALE.Views;

namespace ALE.Http
{
	public class ListenerResponse : IResponse
	{
		protected readonly HttpListenerResponse InnerResponse;

		public IViewProcessor ViewProcessor { get; set; }

		internal ListenerResponse(HttpListenerResponse innerResponse, IContext context)
		{
			if (innerResponse == null) throw new ArgumentNullException("innerResponse");
			if (context == null) throw new ArgumentNullException("context");
			InnerResponse = innerResponse;
			Context = context;
			ContentType = "text/html";
			ContentEncoding = Encoding.UTF8;
		}

		public bool Closed { get; private set; }

		public bool KeepAlive
		{
			get { return InnerResponse.KeepAlive; }
			set { InnerResponse.KeepAlive = value; }
		}

		public Version ProtocalVersion
		{
			get { return InnerResponse.ProtocolVersion; }
			set { InnerResponse.ProtocolVersion = value; }
		}

		public bool SendChunked
		{
			get { return InnerResponse.SendChunked; }
			set { InnerResponse.SendChunked = value; }
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

		public Stream OutputStream
		{
			get { return InnerResponse.OutputStream; }
		}

		public IResponse Write(string text)
		{
			// Construct a response.
			var buffer = ContentEncoding.GetBytes(text);
			return Write(buffer);
		}

		public IResponse Write(byte[] binary)
		{
			OutputStream.Write(binary, 0, binary.Length);
			return this;
		}

		public IResponse Render(string view, object model, Action<Exception> callback = null)
		{
			if (ViewProcessor == null)
			{
				throw new InvalidOperationException("ViewProcessor is null, unable to process view.");
			}
			ViewProcessor.Render(view, model, (ex, rendered) =>
												  {
													  if(ex != null && callback != null)
													  {
														  EventLoop.Pend(t => callback(ex));
													      return;
													  }
													  Write(rendered);
												  });
			return this;
		}

		public void Send()
		{
			OutputStream.Flush();
			OutputStream.Close();
			Context.Complete();
		}

		public CookieCollection Cookies
		{
			get { return InnerResponse.Cookies; }
		}

		public NameValueCollection Headers
		{
			get { return InnerResponse.Headers; }
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

		public void AddHeader(string name, string value)
		{
			InnerResponse.AddHeader(name, value);
		}

		public void AppendCookie(Cookie cookie)
		{
			InnerResponse.AppendCookie(cookie);
		}

		public void AppendHeader(string name, string value)
		{
			InnerResponse.AppendHeader(name, value);
		}

		public void Redirect(string url)
		{
			InnerResponse.Redirect(url);
		}
	}
}