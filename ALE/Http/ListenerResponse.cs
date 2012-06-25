using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace ALE.Http
{
    public class ListenerResponse : IResponse
    {
        protected readonly HttpListenerResponse InnerResponse;

        internal ListenerResponse(HttpListenerResponse innerResponse, IContext context)
        {
            if (innerResponse == null) throw new ArgumentNullException("innerResponse");
            if (context == null) throw new ArgumentNullException("context");
            InnerResponse = innerResponse;
            Context = context;
            ContentType = "text/html";
            ContentEncoding = Encoding.UTF8;
        }

        public IContext Context { get; private set; }

        public bool Closed { get; private set; }

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
            byte[] buffer = ContentEncoding.GetBytes(text);
            return Write(buffer);
        }

        public IResponse Write(byte[] binary)
        {
            OutputStream.Write(binary, 0, binary.Length);
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

        public WebHeaderCollection Headers
        {
            get { return InnerResponse.Headers; }
        }

        public bool KeepAlive
        {
            get
            {
                return InnerResponse.KeepAlive;
            }
            set
            {
                InnerResponse.KeepAlive = value;
            }
        }

        public Version ProtocalVersion
        {
            get
            {
                return InnerResponse.ProtocolVersion;
            }
            set
            {
                InnerResponse.ProtocolVersion = value;
            }
        }

        public string RedirectLocation
        {
            get
            {
                return InnerResponse.RedirectLocation;
            }
            set
            {
                InnerResponse.RedirectLocation = value;
            }
        }

        public int StatusCode
        {
            get
            {
                return InnerResponse.StatusCode;
            }
            set
            {
                InnerResponse.StatusCode = value;
            }
        }

        public string StatusDescription
        {
            get
            {
                return InnerResponse.StatusDescription;
            }
            set
            {
                InnerResponse.StatusDescription = value;
            }
        }

        public bool SendChunked
        {
            get
            {
                return InnerResponse.SendChunked;
            }
            set
            {
                InnerResponse.SendChunked = value;
            }
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
