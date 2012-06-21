using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace ALE.Tcp
{
	public class Net
	{
		public static WebSocketServer CreateServer(Action<WebSocket> callback)
		{
			return new WebSocketServer(callback);
		}
	}

	public class WebSocketServer
	{
		public int Port;
		public string IP;
		public readonly Action<WebSocket> Callback;
		public string Origin;

		public WebSocketServer(Action<WebSocket> callback)
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

		void AcceptTcpClientCallback(IAsyncResult result)
		{
			var listener = (TcpListener)result.AsyncState;

			//get the tcpClient and create a new WebSocket object.
			var tcpClient = listener.EndAcceptTcpClient(result);
			var websocket = new WebSocket(tcpClient);

			//send the handshake
			websocket.Send(GetHandshake(), () =>
			{
				//queue up the callback in the event loop.
				EventLoop.Current.Pend(() => Callback(websocket));
			});

			//listen for the next connection.
			listener.BeginAcceptTcpClient(AcceptTcpClientCallback, listener);
		}

		private string GetHandshake()
		{
			var writer = new StringBuilder();
			writer.AppendLine("HTTP/1.1 101 Web Socket Protocol Handshake");
			writer.AppendLine("Upgrade: WebSocket");
			writer.AppendLine("Connection: Upgrade");
			writer.Append("WebSocket-Origin: " + Origin);
			writer.AppendLine("WebSocket-Location: ws://" + IP + ":" + Port + "/ale");
			writer.AppendLine("");
			return writer.ToString();
		}
	}

	public class WebSocket
	{
		public const int DefaultBufferSize = 8192; //8KB

		public Encoding Encoding = Encoding.UTF8;

		private readonly TcpClient _tcp;
		private byte[] _readBuffer;
		private StringBuilder _readText = new StringBuilder();
		private readonly List<Action<string>> _recieveEvents = new List<Action<string>>();

		public WebSocket(TcpClient tcp)
		{
			_tcp = tcp;
			BeginRead();
		}

		public void Send(string text, Action callback = null)
		{
			var writeBuffer = Encoding.GetBytes(text);
			_tcp.GetStream().BeginWrite(writeBuffer, 0, writeBuffer.Length, SendCallback, callback);
		}

		void SendCallback(IAsyncResult result)
		{
			var callback = result.AsyncState as Action;
			_tcp.GetStream().EndWrite(result);
			if (callback != null)
			{
				EventLoop.Current.Pend(callback);
			}
		}

		public void Receive(Action<string> callback)
		{
			_recieveEvents.Add(callback);
		}

		void BeginRead()
		{
			_tcp.GetStream().BeginRead(_readBuffer, 0, _readBuffer.Length, ReadCallback, _tcp);
		}

		void ReadCallback(IAsyncResult result)
		{
			var tcp = (TcpClient)result.AsyncState;
			var bytesRead = tcp.GetStream().EndRead(result);
			lock (_readText)
			{
				if (bytesRead > 0)
				{
					_readText.Append(Encoding.GetString(_readBuffer, 0, bytesRead));
				}
				else
				{
					var text = _readText.ToString();
					if (text.Length > 0)
					{
						_readText.Clear();
					}
					foreach (var recieveEvent in _recieveEvents)
					{
						EventLoop.Current.Pend(() => recieveEvent(text));
					}
				}
			}
			BeginRead();
		}

	}
}
