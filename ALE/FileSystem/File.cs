using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ALE.FileSystem
{
	public class File
	{
		public const int DefaultBufferSize = 65536; //64KB

		public static void Read(string path, Action<Exception, int, byte[]> callback)
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
				Interlocked.Increment(ref state.ReadIndex);
				var readIndex = state.ReadIndex;
				var buffer = new byte[bytesRead];
				Array.Copy(state.Buffer, buffer, bytesRead);
				var callback = state.Callback;
				EventLoop.Pend(t =>
				               	{
				               		callback(null, readIndex, buffer);
				               	});
			}
		}

		public static void ReadAllBytes(string path, Action<byte[]> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			FileReadAllAsync(path, (buffer) => EventLoop.Pend((t) =>
			                                               	{
			                                               		callback(buffer);
			                                               	}));
		}

		public static void ReadAllText(string path, Action<string> callback)
		{
			ReadAllText(path, Encoding.UTF8, callback);
		}


		public static void ReadAllText(string path, Encoding encoding, Action<string> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			if (encoding == null) throw new ArgumentNullException("encoding");
			FileReadAllAsync(path, (buffer) =>
												{
													var text = encoding.GetString(buffer);
													EventLoop.Pend((t) => callback(text));
												});
		}

		public static void ReadAllLines(string path, Action<string[]> callback)
		{
			ReadAllLines(path, Encoding.UTF8, callback);
		}

		public static void ReadAllLines(string path, Encoding encoding, Action<string[]> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			if (encoding == null) throw new ArgumentNullException("encoding");
			FileReadAllAsync(path, (buffer) =>
												{
													var lines = new List<string>();
													using (var ms = new MemoryStream(buffer))
													using (var reader = new StreamReader(ms))
													{
														while (reader.Peek() >= 0)
														{
															lines.Add(reader.ReadLine());
														}
													}
													EventLoop.Pend((t) => callback(lines.ToArray()));
												});
		}

		private static void FileReadAllAsync(string path, Action<byte[]> complete)
		{
			var fs = System.IO.File.OpenRead(path);
			var buffer = new byte[fs.Length];
			var state = new AsyncFileReadState(complete, fs, buffer);
			fs.BeginRead(buffer, 0, buffer.Length, FileReadCallback, state);
		}

		private static void FileReadCallback(IAsyncResult result)
		{
			var state = (AsyncFileReadState)result.AsyncState;
			state.FileStream.EndRead(result);
			state.Callback(state.Buffer);
		}
	}
}