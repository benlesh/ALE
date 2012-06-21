using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALE
{
	public class Do
	{
		public static void Async(Action operation, Action callback = null)
		{
			EventLoop.Current.Pend(() =>
									{
										operation();
										if (callback != null)
										{
											EventLoop.Current.Pend(callback);
										}
									});
		}
	}
}
