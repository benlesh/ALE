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
    class Program
    {
        static void Main(string[] args)
        {
            EventLoop.Start(() =>
            {
                Server.Create((req, res) =>
                {
                    var bag = req.Context.ContextBag;

                    res.Write("<p>Pre processed data: " + bag.PreProcessedData + "</p>");

                    bag.PostProcessedData = "Bar";
                }).Use(new TestPreprocessor())
                .Use(new TestPostprocessor())
                .Listen("http://localhost:1337/");
            });
            Console.ReadKey();
        }
    }

    class TestPreprocessor : IPreprocessor
    {
        public void Execute(IRequest req, IResponse res)
        {
            var bag = req.Context.ContextBag;
            bag.PreProcessedData = "FOO";
        }
    }


    class TestPostprocessor : IPostprocessor
    {
        public void Execute(IRequest req, IResponse res)
        {
            res.Write("<p>Post processed data: " + req.Context.ContextBag.PostProcessedData + "</p>");
        }
    }
}
