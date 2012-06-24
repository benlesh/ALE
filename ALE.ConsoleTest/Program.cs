using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ALE.Http;
using System.Threading;
using ALE.Tcp;
using System.Diagnostics;

namespace ALE.ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            EventLoop.Start(() =>
            {
                Server.Create((req, res) =>
                {
                    res.Write("<p>Hello World</p>");
                })
                .Use(Middleware.AddFooBefore)
                .Use(Middleware.AddBarAfter)
                .Listen("http://localhost:1337/");
            });
            Console.ReadKey();
        }
    }

    internal class Middleware
    {
        public static PreProcessor AddFooBefore = new PreProcessor(AddFoo);
        public static PostProcessor AddBarAfter = new PostProcessor(AddBar);

        static void AddFoo(IRequest req, IResponse res)
        {
            res.Write("<p>Foo</p>");
        }

        static void AddBar(IRequest req, IResponse res)
        {
            res.Write("<p>Bar</p>");
        }
    }
}
