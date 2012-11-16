using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALE.Http
{
	public abstract class Controller : IController
	{
		public IRequest Request { get; set; }

		public IResponse Response { get; set; }

		public IContext Context { get; set; }

	    protected Controller()
		{
		}
	}
}
