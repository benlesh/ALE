using System;
using System.Threading.Tasks;

namespace ALE.Tcp
{
	public class Net
	{
		public static WebSocketServer CreateServer(Action<Exception, WebSocket> callback)
		{
			return new WebSocketServer(callback);
		}
	}
}