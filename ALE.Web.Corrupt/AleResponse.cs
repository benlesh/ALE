using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALE.Http;
using System.Web;
using System.Collections.Specialized;

namespace ALE.Web
{
	public class AleResponse : IResponse
	{
		public readonly HttpResponse InnerResponse;

		public AleResponse(HttpResponse innerResponse, IContext context)
		{
			if(innerResponse == null) throw new ArgumentNullException("innerResponse");
			if(context == null) throw new ArgumentNullException("context");
			InnerResponse = innerResponse;
			Context = context;
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

		public System.Net.CookieCollection Cookies
		{
			get { throw new NotImplementedException("Sorry"); }
		}

		public NameValueCollection Headers
		{
			get { return InnerResponse.Headers; }
		}

		public System.IO.Stream OutputStream
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
		
		public void AddHeader(string name, string value)
		{
			InnerResponse.AddHeader(name, value);
		}

		public void AppendCookie(System.Net.Cookie cookie)
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
			Context.Complete();
		}

		public void Redirect(string location)
		{
			InnerResponse.Redirect(location);
		}
	}
}
