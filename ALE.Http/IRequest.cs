using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace ALE.Http
{
	public interface IRequest
	{
		IContext Context { get; }
		IEnumerable<string> AcceptTypes { get; }
		Encoding ContentEncoding { get; }
		long ContentLength { get; }
		string ContentType { get; }
		CookieCollection Cookies { get; }
		Stream InputStream { get; }
		string Method { get; }
		NameValueCollection Headers { get; }
		bool IsAuthenticated { get; }
		bool IsLocal { get; }
		Uri Url { get; }
		Uri UrlReferer { get; }
		string UserAgent { get; }
		IEnumerable<string> UserLanguages { get; }
	}
}