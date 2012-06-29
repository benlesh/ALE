using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using System.Threading;
using ALE.Tcp;
using System.Diagnostics;
using ALE.FileSystem;

namespace ALE.ConsoleTest
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			EventLoop.Start((t) =>
			                	{
			                		Routing.Add("/test", typeof (TestController), "Route1");
			                		Routing.Add("/foo/:foo", typeof (TestController), "Route2");
			                		Server.Create()
			                			.Use(Routing.Handler)
			                			.Listen("http://*:1337/");
			                	});
			Console.ReadKey();

		}
	}
}
