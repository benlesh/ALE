using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using ALE.SqlClient;
using System.Threading;

namespace ALE.ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
            var srvr = Server.Create((req, res) => res.Write("Hello World")).Listen("127.0.0.1", 1337);
			Console.WriteLine("press any key to stop.");
			Console.ReadKey();
            srvr.Stop();
		}
	}
}
