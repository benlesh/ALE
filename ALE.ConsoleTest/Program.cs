using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using System.Threading;
using ALE.Tcp;
using System.Diagnostics;
using ALE.FileSystem;

namespace ALE.ConsoleTest
{
    class TestController : Controller
    {
        public void Test()
        {
            Response.Write("TestController.Test");
        }

        public void Foo(string foo)
        {
            Response.Write("TestController.Foo(\"" + foo + "\")");
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Routing.Add("/Test", typeof(TestController), "Test");
            Routing.Add("/Foo/:foo", typeof(TestController), "Foo");
            EventLoop.Start(t => Server.Create()
                .Use(Routing.Handler)
                .Use(Static.Directory("/public"))
                .Listen("http://*:1337/"));
            Console.ReadKey();

        }
    }
}
