using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using System.Threading;
using ALE.Tcp;
using System.Diagnostics;

namespace ALE.ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
            //http://jsfiddle.net/Y3mBp/6/
			EventLoop.Current.WorkerCount = 8;
			EventLoop.Start(() =>
			{
				Net.CreateServer((socket) =>
				{
					socket.Send("blah");
					socket.Receive((text) =>
					{
						Debug.WriteLine("CALLBACK -> " + text);
					});
				}).Listen("127.0.0.1", 1337, "http://fiddle.jshell.net");
			});
			Console.ReadKey();
		}
	}
}
