using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ALE.Http;

namespace ALE.Web
{
	public class AleRequest : IRequest
	{
		public readonly HttpRequest InnerRequest;

		public AleRequest(HttpRequest innerRequest, IContext context)
		{
			if(innerRequest == null) throw new ArgumentNullException("innerRequest");
			if(context == null) throw new ArgumentNullException("context");
			InnerRequest = innerRequest;
			Context = context;
		}

		#region IRequest Members

		public IContext Context { get; private set; }

		public IEnumerable<string> AcceptTypes
		{
			get { return InnerRequest.AcceptTypes; }
		}

		public Encoding ContentEncoding
		{
			get { return InnerRequest.ContentEncoding; }
		}

		public long ContentLength
		{
			get { return InnerRequest.ContentLength; }
		}

		public string ContentType
		{
			get { return InnerRequest.ContentType; }
		}

		public CookieCollection Cookies
		{
			get { throw new NotImplementedException("Sorry, I'm working on this."); }
		}

		public Stream InputStream
		{
			get { return InnerRequest.InputStream; }
		}

		public string Method
		{
			get { return InnerRequest.HttpMethod; }
		}

		public NameValueCollection Headers
		{
			get { return InnerRequest.Headers; }
		}

		public bool IsAuthenticated
		{
			get { return InnerRequest.IsAuthenticated; }
		}

		public bool IsLocal
		{
			get { return InnerRequest.IsLocal; }
		}

		public Uri Url
		{
			get { return InnerRequest.Url; }
		}

		public Uri UrlReferer
		{
			get { return InnerRequest.UrlReferrer; }
		}

		public string UserAgent
		{
			get { return InnerRequest.UserAgent; }
		}

		public IEnumerable<string> UserLanguages
		{
			get { return InnerRequest.UserLanguages; }
		}

		#endregion
	}
}