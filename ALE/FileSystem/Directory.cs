using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ALE
{
    public class Directory
    {
        public static void Exists(string path, Action<bool> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            var run = new Action(() =>
            {
                var exists = System.IO.Directory.Exists(path);
                EventLoop.Current.Pend(() => callback(exists));
            });
            run.BeginInvoke(null, null);
        }

        public static void Create(string path, Action callback = null)
        {
            var run = new Action(() =>
            {
                System.IO.Directory.CreateDirectory(path);
                if (callback != null)
                {
                    EventLoop.Current.Pend(callback);
                }
            });
            run.BeginInvoke(null, null);
        }
        public static void Delete(string path, Action callback = null)
        {
            var run = new Action(() =>
            {
                System.IO.Directory.Delete(path);
                if (callback != null)
                {
                    EventLoop.Current.Pend(callback);
                }
            });
            run.BeginInvoke(null, null);
        }
        public static void GetFiles(string path, Action<string[]> callback)
        {
            GetFiles(path, "*", callback);
        }
        public static void GetFiles(string path, string searchPattern, Action<string[]> callback)
        {
            GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly, callback);
        }
        public static void GetFiles(string path, string searchPattern, SearchOption searchOption, Action<string[]> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            var run = new Action(() =>
            {
                var files = System.IO.Directory.GetFiles(path, searchPattern, searchOption);
                EventLoop.Current.Pend(() => callback(files));
            });
            run.BeginInvoke(null, null);
        }
        public static void EnumerateFiles(string path, Action<string> callback)
        {
            EnumerateFiles(path, "*", callback);
        }
        public static void EnumerateFiles(string path, string searchPattern, Action<string> callback)
        {
            EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly, callback);
        }
        public static void EnumerateFiles(string path, string searchPattern, SearchOption searchOption, Action<string> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            var run = new Action(() =>
            {
                var files = System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption);
                foreach (var file in files)
                {
                    EventLoop.Current.Pend(() => callback(file));
                }
            });
            run.BeginInvoke(null, null);
        }
        public static void GetDirectories(string path, Action<string[]> callback)
        {
            GetDirectories(path, "*", callback);
        }
        public static void GetDirectories(string path, string searchPattern, Action<string[]> callback)
        {
            GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly, callback);
        }
        public static void GetDirectories(string path, string searchPattern, SearchOption searchOption, Action<string[]> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            var run = new Action(() =>
            {
                var dirs = System.IO.Directory.GetDirectories(path, searchPattern, searchOption);
                EventLoop.Current.Pend(() => callback(dirs));
            });
            run.BeginInvoke(null, null);
        }
        public static void EnumerateDirectories(string path, Action<string> callback)
        {
            EnumerateDirectories(path, "*", callback);
        }
        public static void EnumerateDirectories(string path, string searchPattern, Action<string> callback)
        {
            EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly, callback);
        }
        public static void EnumerateDirectories(string path, string searchPattern, SearchOption searchOption, Action<string> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            var run = new Action(() =>
            {
                var files = System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption);
                foreach (var file in files)
                {
                    EventLoop.Current.Pend(() => callback(file));
                }
            });
            run.BeginInvoke(null, null);
        }
    }
}
