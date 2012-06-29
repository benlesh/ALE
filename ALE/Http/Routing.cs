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

		/// <summary>
		/// Handles routed requests.
		/// </summary>
		/// <param name="request">The request to route.</param>
		/// <param name="response">The response to use.</param>
		public static void Handler(IRequest request, IResponse response)
		{
			foreach (var route in Routes)
			{
				if (route.TryExecute(request, response))
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
