using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALE.Http;

namespace ALE.Tests.Http
{
	[TestClass]
	public class Routing_Tests
	{
		[TestMethod]
		public void Route_GetParameters()
		{
			const string path = "/:controller/:action/:id";
			var parameters = Route.GetParameters(path);
			Assert.AreEqual(3, parameters.Length);
			Assert.AreEqual("controller", parameters[0]);
			Assert.AreEqual("action", parameters[1]);
			Assert.AreEqual("id", parameters[2]);
		}

		[TestMethod]
		public void Route_CreatePathTester()
		{
			const string path = "/:controller/:action/:id";
			var pathTester = Route.CreatePathTester(path);
			var pattern = pathTester.ToString();
			Assert.AreEqual(@"^/(?<controller>.+)/(?<action>.+)/(?<id>.+)", pattern);
		}
	}
}
