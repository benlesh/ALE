using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System;
using ALE.Views;

namespace ALE.Http
{
	public interface IResponse
	{
		IContext Context { get; }
		string ContentType { get; set; }
		Encoding ContentEncoding { get; set; }
		CookieCollection Cookies { get; }
		NameValueCollection Headers { get; }
		Stream OutputStream { get; }
		string RedirectLocation { get; set; }
		int StatusCode { get; set; }
		string StatusDescription { get; set; }
	    IViewProcessor ViewProcessor { get; set; }

	    void AddHeader(string name, string value);
		void AppendCookie(Cookie cookie);
		void AppendHeader(string name, string value);
		IResponse Write(string text);
		IResponse Write(byte[] binary);
		void Send();
		void Redirect(string location);
	    IResponse Render(string view, object model, Action<Exception> callback = null);
	}
}