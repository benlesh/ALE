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
		public long ContentLength
		{
			get { return _listenerContext.Response.ContentLength64;  }
			set { _listenerContext.Response.ContentLength64 = Math.Max(0, value); }
		}
		public Stream OutputStream
		{
			get { return _listenerContext.Response.OutputStream; }
		}
		internal Response(HttpListenerContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			_listenerContext = context;
		}

		public Response Write(string output)
		{
			// Construct a response.
			byte[] buffer = Encoding.UTF8.GetBytes(output);
			ContentLength = buffer.Length;
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
