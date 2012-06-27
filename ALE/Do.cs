using System;
using System.Threading.Tasks;

namespace ALE
{
	public class Do
	{
		public static void Async(Action operation, Action<Task> callback = null)
		{
			EventLoop.Pend((t) =>
			                       	{
			                       		operation();
			                       		if (callback != null)
			                       		{
			                       			EventLoop.Pend(callback);
			                       		}
			                       	});
		}
	}
}