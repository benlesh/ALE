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
			CreateRandomFile(@"C:\Foo.txt", 1000000);
			Blah();
			ALEBenchmark();
		}

		private static void CreateRandomFile(string path, int size)
		{
			var buffer = new byte[size];
			System.IO.File.WriteAllBytes(path, buffer);
		}
		private static void SimpleTest()
		{
			EventLoop.Start();
			while (true)
			{
				EventLoop.Pend(t =>
				{
					Console.WriteLine("Test");
				});
				var keyInfo = Console.ReadKey();
				if (keyInfo.Key == ConsoleKey.Escape) break;
			}
			EventLoop.Stop();
		}
		private static void Blah()
		{
			Console.WriteLine("Prepping.");
			GC.Collect();
			GC.WaitForFullGCComplete();
			GC.Collect();

			var sw = new Stopwatch();

			Action complete = () => { Console.WriteLine("Complete: {0} ms", sw.ElapsedMilliseconds); };
			Console.WriteLine("Prepping complete.");
			sw.Start();
			for (int i = 0; i < 10000; i++)
			{
				var x = i;
				var buffer = System.IO.File.ReadAllBytes(@"C:\Foo.txt");
				if (x == 9999)
				{
					complete();
				}
			}
			Console.ReadKey();
		}
		private static void ALEBenchmark()
		{
			Console.WriteLine("Prepping.");
			GC.Collect();
			GC.WaitForFullGCComplete();
			GC.Collect();

			var sw = new Stopwatch();

			EventLoop.WorkerCount = 1;
			Action complete = () => { Console.WriteLine("Complete: {0} ms", sw.ElapsedMilliseconds); };
			EventLoop.Start();
			Console.WriteLine("Prepping complete.");
			sw.Start();
			for (int i = 0; i < 10000; i++)
			{
				var x = i;
				File.ReadAllBytes(@"C:\Foo.txt", (buffer) =>
				                                 	{
				                                 		if (x == 9999) complete();
				                                 	});
			}
			Console.ReadKey();
		}
	}

}
