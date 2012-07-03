using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALE.FileSystem;
using System.IO;
using RazorEngine.Templating;

namespace ALE.Views.Razor
{
    public class RazorView : IViewProcessor
    {
        public static RazorView Default { get { return new RazorView("/Views"); } }
        public static RazorView Processor(string path)
        {
            return new RazorView(path);
        }

        protected readonly string ViewsDirectoryPath;
        protected readonly string ViewsRoot;

        private RazorView(string viewsDirPath)
        {
            ViewsDirectoryPath = viewsDirPath;
            ViewsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     ViewsDirectoryPath.Replace('/', '\\').TrimStart('\\'));
        }

        protected void LoadView(string view, Action<Exception, string> callback)
        {
            var deUrled = view.Replace('/', '\\').TrimStart('\\');
            var viewFile = Path.Combine(ViewsRoot, deUrled);
            if (!System.IO.File.Exists(viewFile))
            {
                throw new FileNotFoundException("View not found.");
            }
            FileSystem.File.ReadAllText(viewFile, (ex, text) => EventLoop.Pend(t => callback(ex, text)));
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
                                       EventLoop.Pend(t => callback(ex2, null));
                                   }
                               });
        }
    }
}
