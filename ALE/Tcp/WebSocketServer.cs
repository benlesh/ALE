using System;
using System.Net;
using System.Net.Sockets;

namespace ALE.Tcp
{
    public class WebSocketServer
    {
        public readonly Action<Exception, WebSocket> Callback;
        public string IP;
        public string Origin;
        public int Port;

        public WebSocketServer(Action<Exception, WebSocket> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            Callback = callback;
        }

        public void Listen(string ip, int port, string origin)
        {
            var address = IPAddress.Parse(ip);
            var listener = new TcpListener(address, port);
            IP = ip;
            Port = port;
            Origin = origin;
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptTcpClientCallback, listener);
        }

        private void AcceptTcpClientCallback(IAsyncResult result)
        {
            var listener = (TcpListener)result.AsyncState;

            //get the tcpClient and create a new WebSocket object.
            var tcpClient = listener.EndAcceptTcpClient(result);
            var websocket = new WebSocket(this, tcpClient);
            websocket.BeginRead();
            //listen for the next connection.
            listener.BeginAcceptTcpClient(AcceptTcpClientCallback, listener);
        }
    }
}