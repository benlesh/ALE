using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace ALE.FileSystem
{
    public class File
    {
        public const int DefaultBufferSize = 65536; //64KB

        /// <summary>
        /// Reads a file as a stream in chunks.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="callback">Called when a chunk is read.</param>
        public static void Read(string path, Action<Exception, long, byte[]> callback)
        {
            var fs = System.IO.File.OpenRead(path);
            var buffer = new byte[DefaultBufferSize];
            var state = new ReadAsyncCallbackState(fs, buffer, callback);
            fs.BeginRead(buffer, 0, buffer.Length, ReadAsyncCallback, state);
        }

        static void ReadAsyncCallback(IAsyncResult result)
        {
            var state = (ReadAsyncCallbackState)result.AsyncState;
            var bytesRead = state.FileStream.EndRead(result);
            if (bytesRead > 0)
            {
                Interlocked.Add(ref state.RemainingBytes, -1*bytesRead);
                var buffer = new byte[bytesRead];
                Array.Copy(state.Buffer, buffer, bytesRead);
                var callback = state.Callback;
                var remainingBytes = state.RemainingBytes;
                EventLoop.Pend(() =>
                                {
                                    callback(null, remainingBytes, buffer);
                                });
            }
        }

        public static void ReadAllBytes(string path, Action<Exception, byte[]> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            FileReadAllAsync(path, (ex, buffer) => EventLoop.Pend(() => callback(ex, buffer)));
        }

        public static void ReadAllText(string path, Action<Exception, string> callback)
        {
            ReadAllText(path, Encoding.UTF8, callback);
        }

        public static void ReadAllText(string path, Encoding encoding, Action<Exception, string> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            if (encoding == null) throw new ArgumentNullException("encoding");
            FileReadAllAsync(path, (ex, buffer) =>
                                                {
                                                    try
                                                    {
                                                        if (ex != null) throw ex;
                                                        var text = encoding.GetString(buffer);
                                                        EventLoop.Pend(() => callback(null, text));
                                                    } catch (Exception ex2)
                                                    {
                                                        EventLoop.Pend(() => callback(ex2, null));
                                                    }
                                                });
        }

        public static void ReadAllLines(string path, Action<Exception, string[]> callback)
        {
            ReadAllLines(path, Encoding.UTF8, callback);
        }

        public static void ReadAllLines(string path, Encoding encoding, Action<Exception, string[]> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            if (encoding == null) throw new ArgumentNullException("encoding");
            FileReadAllAsync(path, (ex, buffer) =>
                                                {
                                                    try
                                                    {
                                                        if (ex != null)
                                                        {
                                                            throw ex;
                                                        }
                                                        var lines = new List<string>();
                                                        using (var ms = new MemoryStream(buffer))
                                                        using (var reader = new StreamReader(ms))
                                                        {
                                                            while (reader.Peek() >= 0)
                                                            {
                                                                lines.Add(reader.ReadLine());
                                                            }
                                                        }
                                                        EventLoop.Pend(() => callback(null, lines.ToArray()));
                                                    } catch (Exception ex2)
                                                    {
                                                        EventLoop.Pend(() => callback(ex2, null));
                                                    }
                                                });
        }

        private static void FileReadAllAsync(string path, Action<Exception, byte[]> complete)
        {
            try
            {
                var fs = System.IO.File.OpenRead(path);
                var buffer = new byte[fs.Length];
                var state = new AsyncFileReadState(complete, fs, buffer);
                fs.BeginRead(buffer, 0, buffer.Length, FileReadAllCallback, state);
            } catch (Exception ex)
            {
                EventLoop.Pend(() => complete(ex, null));
            }
        }

        private static void FileReadAllCallback(IAsyncResult result)
        {
            var state = (AsyncFileReadState)result.AsyncState;
            state.FileStream.EndRead(result);
            state.Callback(null, state.Buffer);
        }
    }
}