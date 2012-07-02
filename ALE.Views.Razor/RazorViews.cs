using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALE.FileSystem;

namespace ALE.Views.Razor
{
    public class RazorViews : IViewProcessor
    {
        public static RazorViews Default { get { return new RazorViews(); } }

        private RazorViews()
        {

        }

        protected void LoadView(string view, Action<Exception, string> callback)
        {
            var path = view; //TODO: add lookcup system.
            File.ReadAllText(path, (ex, text) => EventLoop.Pend(t => callback(ex, text)));
        }
        public void Render(string view, object model, Action<Exception, string> callback)
        {
            LoadView(view, (ex, viewtext) =>
                               {
                                   if (ex != null)
                                   {
                                       EventLoop.Pend(t => callback(ex, null));
                                   }
                                   try
                                   {
                                       var result = RazorEngine.Razor.Parse(viewtext, model);
                                       EventLoop.Pend(t => callback(null, result));
                                   } catch (Exception ex2)
                                   {
                                       EventLoop.Pend(t => callback(ex, null));
                                   }
                               });
        }
    }
}
