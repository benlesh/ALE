using System;
using System.Collections.Generic;
using System.Text;
using MS.Internal.Xml.XPath;
using System.Linq.Expressions;
using System.Reflection;

namespace ALE.Http
{
	public class Routing
	{
		/// <summary>
		/// Holds all routes to be checked.
		/// </summary>
		public static readonly List<Route> Routes = new List<Route>();

		public static void Handler(IContext context, Action next)
		{
			foreach (var route in Routes)
			{
				if (route.TryExecute(context, next))
				{
					break;
				}
			}
		}

		public static void Add(string path, Type controllerType, string methodName)
		{
			Routes.Add(new Route(path, methodName, controllerType));
		}
	}
}
