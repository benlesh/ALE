using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ALE.Tcp
{
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
        }

        public void Send(string text, Action<Exception> callback = null)
        {
            Debug.WriteLine(text);
            byte[] writeBuffer = EncodeServerData(text);
            NetworkStream netstream = _tcp.GetStream();
            var state = new SendState(netstream, callback);
            netstream.BeginWrite(writeBuffer, 0, writeBuffer.Length, SendCallback, state);
        }

        public void SendUnencoded(string text, Action<Exception> callback = null)
        {
            byte[] writeBuffer = Encoding.GetBytes(text);
            NetworkStream netstream = _tcp.GetStream();
            var state = new SendState(netstream, callback);
            netstream.BeginWrite(writeBuffer, 0, writeBuffer.Length, SendCallback, state);
        }

        private void SendCallback(IAsyncResult result)
        {
            var state = (SendState) result.AsyncState;
            NetworkStream netstream = state.NetworkStream;
            Action<Exception> callback = state.Callback;
            try
            {
                netstream.EndWrite(result);
                if (callback != null)
                {
                    EventLoop.Pend(() => callback(null));
                }
            }
            catch (Exception ex)
            {
                EventLoop.Pend(() => callback(ex));
            }
        }

        public void Receive(Action<string> callback)
        {
            _receiveEvents.Add(callback);
        }

        internal void BeginRead()
        {
            NetworkStream netstream = _tcp.GetStream();
            int bufferSize = _tcp.ReceiveBufferSize;
            var state = new ReadState(netstream, bufferSize);
            netstream.BeginRead(state.Buffer, 0, state.Buffer.Length, ReadCallback, state);
        }

        private void ReadCallback(IAsyncResult result)
        {
            var state = (ReadState) result.AsyncState;
            NetworkStream netstream = state.NetworkStream;
            int bytesRead = netstream.EndRead(result);
            ProcessRead(bytesRead, state);
        }

        private void ProcessRead(int bytesRead, ReadState state)
        {
            if (bytesRead > 0)
            {
                byte[] buffer = state.Buffer;
                if (String.IsNullOrEmpty(ClientHandshake))
                {
                    SendHandshake(buffer, bytesRead);
                }
                else
                {
                    ProcessIncoming(buffer, bytesRead);
                    BeginRead();
                }
            }
        }

        public void ProcessIncoming(byte[] buffer, int bytesRead)
        {
            string text = DecodeClientData(buffer.Take(bytesRead).ToArray());
            foreach (var receive in _receiveEvents)
            {
                Action<string> rec = receive;
                EventLoop.Pend(() => rec(text));
            }
        }

        private void SendHandshake(byte[] buffer, int bytesRead)
        {
            string text = Encoding.GetString(buffer, 0, bytesRead);
            ClientHandshake = text;
            string secKey = WebSocketHandshake.GetSecKey(ClientHandshake);
            SendUnencoded(WebSocketHandshake.GetHandshake(Encoding, secKey, Server.IP, Server.Port, Server.Origin),
                          ex =>
                              {
                                  if (ex != null)
                                  {
                                      EventLoop.Pend(() => Server.Callback(ex, null));
                                  }
                                  BeginRead();
                                  EventLoop.Pend(() => Server.Callback(null, this));
                              });
        }

        private byte[] EncodeServerData(string text)
        {
            byte[] bytesRaw = Encoding.GetBytes(text);
            byte[] header;
            if (bytesRaw.Length <= 125)
            {
                header = new byte[]
                             {
                                 129,
                                 (byte) bytesRaw.Length
                             };
            }
            else if (bytesRaw.Length >= 126 && bytesRaw.Length <= 65535)
            {
                header = new byte[]
                             {
                                 129,
                                 126,
                                 (byte) ((bytesRaw.Length >> 8) & 255),
                                 (byte) (bytesRaw.Length & 255),
                             };
            }
            else
            {
                header = new byte[]
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
            byte secondByte = bytes[1];
            //length = secondByte AND 127 // may not be the actual length in the two special cases
            int length = secondByte & 127;
            //indexFirstMask = 2          // if not a special case
            int indexFirstMask = 2;
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
            byte[] masks = bytes.Skip(indexFirstMask).Take(4).ToArray();
            //indexFirstDataByte = indexFirstMask + 4 // four bytes further
            int indexFirstDataByte = indexFirstMask + 4;
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

        public class ReadState
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
            public readonly Action<Exception> Callback;
            public readonly NetworkStream NetworkStream;

            public SendState(NetworkStream netstream, Action<Exception> callback)
            {
                NetworkStream = netstream;
                Callback = callback;
            }
        }

        #endregion
    }
}