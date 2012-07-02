using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ALE.FileSystem
{
    public class FileSystemWatcher : IDisposable
    {
        private readonly System.IO.FileSystemWatcher _fsw;

        public string Filter
        {
            get { return _fsw.Filter; }
            set { _fsw.Filter = value; }
        }

        public string Path
        {
            get { return _fsw.Path; }
            set { _fsw.Path = value; }
        }

        public static FileSystemWatcher Watch(string path, string filter = null)
        {
            var fsw = new FileSystemWatcher(path, filter);
            return fsw.Start();
        }

        private FileSystemWatcher(string path, string filter)
        {
            if (!String.IsNullOrWhiteSpace(filter))
            {
                _fsw = new System.IO.FileSystemWatcher(path, filter);
            } else
            {
                _fsw = new System.IO.FileSystemWatcher(path);
            }
        }

        public FileSystemWatcher Changed(Action<string> callback)
        {
            _fsw.Changed += (sender, args) => EventLoop.Pend(t => callback(args.FullPath));
            return this;
        }

        public FileSystemWatcher Deleted(Action<string> callback)
        {
            _fsw.Deleted += (sender, args) => EventLoop.Pend(t => callback(args.FullPath));
            return this;
        }

        public FileSystemWatcher Created(Action<string> callback)
        {
            _fsw.Created += (sender, args) => EventLoop.Pend(t => callback(args.FullPath));
            return this;
        }

        public FileSystemWatcher Renamed(Action<string, string> callback)
        {
            _fsw.Renamed += (sender, args) => EventLoop.Pend(t => callback(args.FullPath, args.OldFullPath));
            return this;
        }

        public FileSystemWatcher Start()
        {
            _fsw.EnableRaisingEvents = true;
            return this;
        }

        public FileSystemWatcher Stop()
        {
            _fsw.EnableRaisingEvents = false;
            return this;
        }

        ~FileSystemWatcher()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_fsw != null)
            {
                _fsw.Dispose();
            }
        }
    }
}
