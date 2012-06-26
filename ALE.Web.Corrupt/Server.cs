using System;
using ALE.Http;

namespace ALE.Web
{
	public class Server
	{
		public event Action<IRequest, IResponse> Process;

		private static Server _instance;

		public static Server Create()
		{
			if (_instance == null)
			{
				_instance = new Server();
			}
			return _instance;
		}

		Server()
		{
		}

		public Server Use(Action<IRequest, IResponse> processor)
		{
			Process += processor;
			return this;
		}

		internal void Execute(IRequest req, IResponse res)
		{
			foreach (var processor in Process.GetInvocationList())
			{
				processor.DynamicInvoke(req, res);
				if (req.Context.IsExecutionComplete) break;
			}
		}
	}
}