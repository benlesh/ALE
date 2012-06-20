using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALE
{
	public class ConsoleIO
	{
		public static void ReadLine(Action<string> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			string line = String.Empty;
			Action readLine = () =>
								{
									line = System.Console.ReadLine();
								};
			readLine.BeginInvoke((ar) => EventLoop.Current.Pend(() => callback(line)), null);
		}

		public static void ReadKey(Action<ConsoleKeyInfo> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
			Action readLine = () =>
			{
				keyInfo = System.Console.ReadKey();
			};
			readLine.BeginInvoke((ar) => EventLoop.Current.Pend(() => callback(keyInfo)), null);
		}
	}
}
