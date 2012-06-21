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
			EventLoop.Current.WorkerCount = 8;
			EventLoop.Start(() =>
			{
				for (int i = 0; i < 10000; i++)
				{
					var c = i;
					Do.Async(() =>
								{
									var b = 0;
									for (int j = 0; j < 1000000; j++)
									{
										b *= j;
									}
								}, () => Console.WriteLine(c));
				}
			});
			Console.ReadKey();
		}
	}
}
