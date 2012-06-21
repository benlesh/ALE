using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using System.Threading;

namespace ALE.ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			EventLoop.Start(() =>
			{
				Server.Create((req, res) =>
				{
					File.ReadAllText(@"C:\Foo.log", (text) =>
					{
						res.Write("This is the Foo.log: <br/>")
							.Write("<div>").Write(text).Send("</div>");
					});
				}).Listen("http://*:1337/");
				Console.WriteLine("Server is running.");
			});
			Console.ReadKey();
		}
	}
}
