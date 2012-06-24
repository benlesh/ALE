using System;
using System.IO;
using System.Net;
using System.Text;

namespace ALE.Http
{
    public interface IResponse
    {
        IContext Context { get; }
        string ContentType { get; set; }
        Encoding ContentEncoding { get; set; }
        CookieCollection Cookies { get; }
        WebHeaderCollection Headers { get; }
        bool KeepAlive { get; set; }
        Stream OutputStream { get; }
        Version ProtocalVersion { get; set; }
        string RedirectLocation { get; set; }
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        bool SendChunked { get; set; }

        void AddHeader(string name, string value);
        void AppendCookie(Cookie cookie);
        void AppendHeader(string name, string value);
        IResponse Write(string text);
        IResponse Write(byte[] binary);
        void Close(string text = "");
        void Redirect(string location);
    }
}