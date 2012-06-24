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
                Server.Create()
                    .Use((req, res) => res.Write("<p>Foo!</p>"))
                    .Use((req, res) => res.Write("<p>Bar!</p>"))
                    .Use((req, res) => res.Write("<p>Wassup!</p>"))
                    .Listen("http://*:1337/");
            });
            Console.ReadKey();
        }
    }

}
