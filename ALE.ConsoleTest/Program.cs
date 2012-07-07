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
using ALE.Views.Razor;

namespace ALE.ConsoleTest
{
    class TestController : Controller
    {
        public void Test(Action next)
        {
            Response.Write("TestController.Test");
            next();
        }

        public void Foo(Action next, string foo)
        {
            Response.Render("ViewTest.cshtml", new TestModel {Title = foo}, (ex) => next());
        }
    }

    public class TestModel
    {
        public string Title { get; set; }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            Routing.Add("/Test", typeof(TestController), "Test");
            Routing.Add("/Foo/:foo", typeof(TestController), "Foo");
            EventLoop.Start(() => Server.Create()
                .Use(RazorView.Default)
                .Use(Routing.Handler)
                .Use(Static.Directory("/public"))
                .Listen("http://*:1337/"));
            Console.ReadKey();

        }
    }
}
