using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
		public readonly Action<WebSocket> Callback;
		public string IP;
		public string Origin;
		public int Port;

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

		private void AcceptTcpClientCallback(IAsyncResult result)
		{
			var listener = (TcpListener) result.AsyncState;

			//get the tcpClient and create a new WebSocket object.
			var tcpClient = listener.EndAcceptTcpClient(result);
			var websocket = new WebSocket(this, tcpClient);

			//listen for the next connection.
			listener.BeginAcceptTcpClient(AcceptTcpClientCallback, listener);
		}
	}

	public class WebSocket
	{
		public readonly WebSocketServer Server;
		private readonly List<Action<string>> _receiveEvents = new List<Action<string>>();
		private readonly TcpClient _tcp;
		public string ClientHandshake;
		public Encoding Encoding = Encoding.Default;

		public WebSocket(WebSocketServer server, TcpClient tcp)
		{
			Server = server;
			_tcp = tcp;
			BeginRead();
		}

		private string GetHandshake(string secKey)
		{
			var writer = new StringBuilder();
			writer.AppendLine("HTTP/1.1 101 Web Socket Protocol Handshake");
			writer.AppendLine("Upgrade: websocket");
			writer.AppendLine("Connection: Upgrade");
			writer.AppendLine("WebSocket-Origin: " + Server.Origin);
			writer.AppendLine("WebSocket-Location: ws://" + Server.IP + ":" + Server.Port + "/ale");
			if (!String.IsNullOrEmpty(secKey))
			{
				writer.AppendLine("Sec-WebSocket-Accept: " + HashSecKey(secKey));
			}
			writer.AppendLine("");
			return writer.ToString();
		}

		private string GetSecKey(string clientHandshake)
		{
			var regSecKey = new Regex(@"Sec-WebSocket-Key: (.*?)\r\n");
			var match = regSecKey.Match(clientHandshake);
			if (match == null) return String.Empty;
			return match.Groups[1].Value;
		}

		private string HashSecKey(string secKey)
		{
			using (var sha1 = new SHA1Managed())
			{
				var hash = sha1.ComputeHash(Encoding.GetBytes(secKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
				return Convert.ToBase64String(hash);
			}
		}

		public void Send(string text, Action callback = null)
		{
			Debug.WriteLine(text);
			var writeBuffer = EncodeServerData(text);
			var netstream = _tcp.GetStream();
			var state = new SendState(netstream, callback);
			netstream.BeginWrite(writeBuffer, 0, writeBuffer.Length, SendCallback, state);
		}

		public void SendUnencoded(string text, Action callback = null)
		{
			Debug.WriteLine(text);
			var writeBuffer = Encoding.GetBytes(text);
			var netstream = _tcp.GetStream();
			var state = new SendState(netstream, callback);
			netstream.BeginWrite(writeBuffer, 0, writeBuffer.Length, SendCallback, state);
		}

		private void SendCallback(IAsyncResult result)
		{
			Debug.WriteLine("---- Sent bytes.");
			var state = (SendState) result.AsyncState;
			var netstream = state.NetworkStream;
			var callback = state.Callback;
			netstream.EndWrite(result);
			if (callback != null)
			{
				EventLoop.Current.Pend(callback);
			}
		}

		public void Receive(Action<string> callback)
		{
			_receiveEvents.Add(callback);
		}

		private void BeginRead()
		{
			var netstream = _tcp.GetStream();
			var state = new ReadState(netstream, _tcp.ReceiveBufferSize);
			netstream.BeginRead(state.Buffer, 0, state.Buffer.Length, ReadCallback, state);
		}

		private void ReadCallback(IAsyncResult result)
		{
			var state = (ReadState) result.AsyncState;
			var netstream = state.NetworkStream;
			var bytesRead = netstream.EndRead(result);
			if (bytesRead > 0)
			{
				if (String.IsNullOrEmpty(ClientHandshake))
				{
					var text = Encoding.GetString(state.Buffer, 0, bytesRead);
					ClientHandshake = text;
					var secKey = GetSecKey(ClientHandshake);
					SendUnencoded(GetHandshake(secKey), () =>
					                                    	{
					                                    		BeginRead();
					                                    		EventLoop.Current.Pend(() => Server.Callback(this));
					                                    	});
				}
				else
				{
					var text = DecodeClientData(state.Buffer.Take(bytesRead).ToArray());
					Debug.WriteLine("Read: " + text);
					Debug.WriteLine("Events to fire: " + _receiveEvents.Count);
					foreach (var receive in _receiveEvents)
					{
						EventLoop.Current.Pend(() => receive(text));
					}
					BeginRead();
				}
			}
		}

		private byte[] EncodeServerData(string text)
		{
			var bytesRaw = Encoding.GetBytes(text);
			byte[] header;
			if (bytesRaw.Length <= 125)
			{
				header = new byte[2]
				         	{
				         		129,
				         		(byte) bytesRaw.Length
				         	};
			}
			else if (bytesRaw.Length >= 126 && bytesRaw.Length <= 65535)
			{
				header = new byte[4]
				         	{
				         		129,
				         		126,
				         		(byte) ((bytesRaw.Length >> 8) & 255),
				         		(byte) (bytesRaw.Length & 255),
				         	};
			}
			else
			{
				header = new byte[10]
				         	{
				         		129,
				         		127,
				         		(byte) ((bytesRaw.Length >> 56) & 255),
				         		(byte) ((bytesRaw.Length >> 48) & 255),
				         		(byte) ((bytesRaw.Length >> 40) & 255),
				         		(byte) ((bytesRaw.Length >> 32) & 255),
				         		(byte) ((bytesRaw.Length >> 24) & 255),
				         		(byte) ((bytesRaw.Length >> 16) & 255),
				         		(byte) ((bytesRaw.Length >> 8) & 255),
				         		(byte) (bytesRaw.Length & 255)
				         	};
			}
			var result = new byte[header.Length + bytesRaw.Length];
			header.CopyTo(result, 0);
			Array.ConstrainedCopy(bytesRaw, 0, result, header.Length, bytesRaw.Length);
			return result;
		}

		private string DecodeClientData(byte[] bytes)
		{
			//            secondByte = bytes[1]
			var secondByte = bytes[1];
			//length = secondByte AND 127 // may not be the actual length in the two special cases
			var length = secondByte & 127;
			//indexFirstMask = 2          // if not a special case
			var indexFirstMask = 2;
			//if length == 126            // if a special case, change indexFirstMask
			//    indexFirstMask = 4
			if (length == 126)
			{
				indexFirstMask = 4;
			}
				//else if length == 127       // ditto
				//    indexFirstMask = 10
			else if (length == 127)
			{
				indexFirstMask = 10;
			}
			//masks = bytes.slice(indexFirstMask, 4) // four bytes starting from indexFirstMask
			var masks = bytes.Skip(indexFirstMask).Take(4).ToArray();
			//indexFirstDataByte = indexFirstMask + 4 // four bytes further
			var indexFirstDataByte = indexFirstMask + 4;
			//decoded = new array
			var decoded = new byte[bytes.Length - indexFirstDataByte];
			//decoded.length = bytes.length - indexFirstDataByte // length of real data

			//for i = indexFirstDataByte, j = 0; i < bytes.length; i++, j++
			//    decoded[j] = bytes[i] XOR masks[j MOD 4]
			for (int i = indexFirstDataByte, j = 0; i < bytes.Length; i++, j++)
			{
				decoded[j] = (byte) (bytes[i] ^ masks[j%4]);
			}

			return Encoding.GetString(decoded);
		}

		#region Nested type: ReadState

		private class ReadState
		{
			public readonly byte[] Buffer;
			public readonly NetworkStream NetworkStream;
			public readonly StringBuilder Text;

			public ReadState(NetworkStream netstream, int bufferSize)
			{
				NetworkStream = netstream;
				Buffer = new byte[bufferSize];
				Text = new StringBuilder();
			}

			public bool HasText
			{
				get { return Text.Length > 0; }
			}
		}

		#endregion

		#region Nested type: SendState

		private class SendState
		{
			public readonly Action Callback;
			public readonly NetworkStream NetworkStream;

			public SendState(NetworkStream netstream, Action callback)
			{
				NetworkStream = netstream;
				Callback = callback;
			}
		}

		#endregion
	}
}