using ALE.Http;

namespace ALE.ConsoleTest
{
	public class TestController : Controller
	{
		public void Route1()
		{
			Response.Write("Route1 found!");
		}

		public void Route2(int foo)
		{
			Response.Write("Foo: " + foo);
		}
	}
}