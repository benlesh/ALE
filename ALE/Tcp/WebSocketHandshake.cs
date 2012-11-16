using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ALE.Tcp
{
    public static class WebSocketHandshake
    {
        public static string GetHandshake(Encoding enc, string secKey, string ip, int port, string origin)
        {
            var writer = new StringBuilder();
            writer.AppendLine("HTTP/1.1 101 Web Socket Protocol Handshake");
            writer.AppendLine("Upgrade: websocket");
            writer.AppendLine("Connection: Upgrade");
            writer.AppendLine("WebSocket-Origin: " + origin);
            writer.AppendLine("WebSocket-Location: ws://" + ip + ":" + port + "/ale");
            if (!String.IsNullOrEmpty(secKey))
            {
                writer.AppendLine("Sec-WebSocket-Accept: " + HashSecKey(enc, secKey));
            }
            writer.AppendLine("");
            return writer.ToString();
        }

        public static string GetSecKey(string clientHandshake)
        {
            var regSecKey = new Regex(@"Sec-WebSocket-Key: (.*?)\r\n");
            var match = regSecKey.Match(clientHandshake);
            if (match == null) return String.Empty;
            return match.Groups[1].Value;
        }

        public static string HashSecKey(Encoding enc, string secKey)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(enc.GetBytes(secKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
                return Convert.ToBase64String(hash);
            }
        }
    }
}