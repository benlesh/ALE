using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using System.Threading;
using ALE.Tcp;

namespace ALE.ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			EventLoop.Current.WorkerCount = 8;
			EventLoop.Start(() =>
								{
									Net.CreateServer((socket) =>
															{
																socket.Receive(Console.WriteLine);
															}).Listen("127.0.0.1", 1337, "http://jsfiddle.net");
								});
			Console.ReadKey();
		}
	}
}
