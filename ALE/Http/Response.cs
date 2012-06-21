using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ALE.Http
{
	public class Response : IDisposable
	{
		private readonly HttpListenerContext _listenerContext;

		public HttpListenerContext ListenerContext
		{
			get { return _listenerContext; }
		}


		public string ContentType
		{
			get { return _listenerContext.Response.ContentType; }
			set { _listenerContext.Response.ContentType = value; }
		}

		public Stream OutputStream
		{
			get { return _listenerContext.Response.OutputStream; }
		}

		internal Response(HttpListenerContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			_listenerContext = context;
			ContentType = "text/html";
		}

		public Response Write(string output)
		{
			// Construct a response.
			byte[] buffer = Encoding.UTF8.GetBytes(output);
			OutputStream.Write(buffer, 0, buffer.Length);
			return this;
		}

		public void Send(string output = "")
		{
			if (!String.IsNullOrWhiteSpace(output))
			{
				Write(output);
			}
			OutputStream.Close();
		}

		~Response()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_listenerContext != null && _listenerContext.Response != null && _listenerContext.Response.OutputStream != null)
			{
				_listenerContext.Response.OutputStream.Close();
				_listenerContext.Response.OutputStream.Dispose();
			}
		}
	}
}
