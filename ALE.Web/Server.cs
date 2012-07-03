using System;
using ALE.Http;
using ALE.Views;

namespace ALE.Web
{
	public class Server : ServerBase
	{
		private static Server _instance;

		private Server()
		{
		}

		public static Server Create()
		{
			if (_instance == null)
			{
				_instance = new Server();
			}
			return _instance;
		}

		public new Server Use(Action<IContext, Action> processor)
		{
			return (Server) base.Use(processor);
		}

		public new Server Use(IViewProcessor viewProcessor)
		{
			return (Server) base.Use(viewProcessor);
		}

		internal void Execute(IContext context)
		{
		    this.OnProcess(context);
		}
	}
}