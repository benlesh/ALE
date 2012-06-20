using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ALE
{
    public class FileIO
    {
        public static void Exists(string path, Action<bool> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            new Action(() =>
            {
                var exists = System.IO.File.Exists(path);
                EventLoop.Current.Pend(() => callback(exists));
            }).BeginInvoke(null, null);
        }
        public static void ReadAllBytes(string path, Action<byte[]> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            new Action(() =>
            {
                var data = System.IO.File.ReadAllBytes(path);
                EventLoop.Current.Pend(() => callback(data));
            }).BeginInvoke(null, null);
        }
        public static void ReadAllLines(string path, Action<string[]> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            new Action(() => {
                var lines = System.IO.File.ReadAllLines(path);
                EventLoop.Current.Pend(() => callback(lines));
            }).BeginInvoke(null, null);
        }
        public static void ReadAllText(string path, Action<string> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            new Action(() =>
            {
                var text = System.IO.File.ReadAllText(path);
                EventLoop.Current.Pend(() => callback(text));
            }).BeginInvoke(null, null);
        }
        public static void ReadLines(string path, Action<string> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            new Action(() =>
            {
                var lines = System.IO.File.ReadLines(path);
                foreach (var line in lines)
                {
                    EventLoop.Current.Pend(() => callback(line));
                }
            }).BeginInvoke(null, null);
        }
        public static void Replace(string source, string destination, string destinationBackup, Action callback = null)
        {
            new Action(() => {
                System.IO.File.Replace(source, destination, destinationBackup);
                if (callback != null)
                {
                    EventLoop.Current.Pend(callback);
                }
            }).BeginInvoke(null, null);
        }
        public static void Copy(string source, string destination, Action callback = null)
        {
            new Action(() => {
                System.IO.File.Copy(source, destination);
                if (callback != null)
                {
                    EventLoop.Current.Pend(callback);
                }
            }).BeginInvoke(null, null);
        }
        public static void Delete(string path, Action callback = null)
        {
            new Action(() =>
            {
                System.IO.File.Delete(path);
                if (callback != null)
                {
                    EventLoop.Current.Pend(callback);
                }
            }).BeginInvoke(null, null);
        }
    }
}
