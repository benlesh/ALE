using System;
using System.Linq;
using System.Net;
using ALE.Views;
using System.Collections.Generic;

namespace ALE.Http
{
    /// <summary>
    ///   A simple asynchronous web server for ALE.
    /// </summary>
    public class Server : ServerBase
    {
        /// <summary>
        ///   The HTTP listener that handles receiving requests.
        /// </summary>
        protected readonly HttpListener Listener;

        /// <summary>
        ///   Creates a new instance of a server.
        /// </summary>
        /// <param name="callback"> The request received delegate. </param>
        private Server()
        {
            Listener = new HttpListener();
        }

        /// <summary>
        ///   Starts the server listening an any number of URI prefixes.
        /// </summary>
        /// <param name="prefixes"> URI prefixes for the server to listen on. </param>
        /// <returns> The server it started. </returns>
        public Server Listen(params string[] prefixes)
        {
            if (Process == null)
                throw new InvalidOperationException("Cannot run a server with no callback. Process event must be set.");
            foreach (var prefix in prefixes)
            {
                Listener.Prefixes.Add(prefix);
            }
            if (!Listener.IsListening)
            {
                Listener.Start();
                Listener.BeginGetContext(GetContextCallback, Listener);
            }
            return this;
        }

        /// <summary>
        ///   Async callback for GetContextCallBack in Listen method above.
        /// </summary>
        /// <param name="result"> The IAsyncResult. </param>
        private void GetContextCallback(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            var resultContext = listener.EndGetContext(result);

            var context = new ListenerContext(resultContext);
            EventLoop.Pend(() =>
                                    {
                                        if (ViewProcessor != null)
                                        {
                                            context.Response.ViewProcessor = ViewProcessor;
                                        }
                                        OnProcess(context);
                                    });
            listener.BeginGetContext(GetContextCallback, listener);
        }

        /// <summary>
        ///   Stop the server.
        /// </summary>
        /// <param name="stopEventLoop"> </param>
        public void Stop(bool stopEventLoop = false)
        {
            Listener.Stop();
            if (stopEventLoop)
            {
                EventLoop.Stop();
            }
        }

        public new Server Use(Action<IContext, Action> processor)
        {
            return (Server)base.Use(processor);
        }

        public new Server Use(IViewProcessor viewProcessor)
        {
            return (Server)base.Use(viewProcessor);
        }

        public static Server Create()
        {
            return new Server();
        }
    }

    public abstract class ServerBase
    {

        /// <summary>
        /// Sets the type of view processor to use.
        /// </summary>
        /// <param name="viewProcessor">The view processor to use.</param>
        /// <returns>The server instance.</returns>
        public ServerBase Use(IViewProcessor viewProcessor)
        {
            ViewProcessor = viewProcessor;
            return this;
        }

        /// <summary>
        /// The view processor requests will use.
        /// </summary>
        protected IViewProcessor ViewProcessor;

        /// <summary>
        ///   Adds preprocessing middleware.
        /// </summary>
        /// <param name="middleware"> The middleware to add. </param>
        /// <param name="processor"> </param>
        /// <returns> The server instance. </returns>
        public ServerBase Use(Action<IContext, Action> processor)
        {
            Process.Add(processor);
            return this;
        }

        public readonly List<Action<IContext, Action>> Process = new List<Action<IContext, Action>>();

        protected void OnProcess(IContext context)
        {
            var processQueue = new Queue<Action<IContext, Action>>(Process);
            Action next = null;
            next = () =>
            {
                if (processQueue.Count > 0)
                {
                    var nextProcessor = processQueue.Dequeue();
                    if (nextProcessor != null)
                    {
                        nextProcessor(context, next);
                    }
                } else
                {
                    context.Response.Send();
                }
            };
            next();
        }
    }
}