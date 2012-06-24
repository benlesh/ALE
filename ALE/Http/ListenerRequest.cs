using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace ALE.Http
{
    internal class ListenerRequest : IRequest
    {
        protected readonly HttpListenerRequest InnerRequest;

        private readonly IContext _context;
        
        internal ListenerRequest(HttpListenerRequest innerRequest, IContext context)
        {
            if (innerRequest == null) throw new ArgumentNullException("innerRequest");
            if (context == null) throw new ArgumentNullException("context");
            InnerRequest = innerRequest;
            _context = context;
        }

        public IContext Context
        {
            get { return _context; }
        }

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
            get { return InnerRequest.ContentLength64; }
        }

        public string ContentType
        {
            get { return InnerRequest.ContentType; }
        }

        public CookieCollection Cookies
        {
            get { return InnerRequest.Cookies; }
        }

        public Stream InputStream
        {
            get { return InnerRequest.InputStream; }
        }

        public string Method
        {
            get { return InnerRequest.HttpMethod; }
        }

        public bool HasBody
        {
            get { return InnerRequest.HasEntityBody; }
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
    }
}
