using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ALE
{
	public class File
	{
		public const int DefaultBufferSize16K = 16384; //16KB

		public static void ReadAllBytes(string path, Action<byte[]> callback)
		{
			ReadAllBytes(path, DefaultBufferSize16K, callback);
		}
		public static void ReadAllBytes(string path, int bufferSize, Action<byte[]> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			FileReadAsync(path, bufferSize, (state) =>
												{
													var data = state.OutputStream.ToArray();
													EventLoop.Current.Pend(() => callback(data));
												});
		}

		public static void ReadAllText(string path, Action<string> callback)
		{
			ReadAllText(path, Encoding.UTF8, DefaultBufferSize16K, callback);
		}
		public static void ReadAllText(string path, Encoding encoding, Action<string> callback)
		{
			ReadAllText(path, encoding, DefaultBufferSize16K, callback);
		}
		public static void ReadAllText(string path, Encoding encoding, int bufferSize, Action<string> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			if (encoding == null) throw new ArgumentNullException("encoding");
			FileReadAsync(path, bufferSize, (state) =>
												{
													var text = encoding.GetString(state.OutputStream.ToArray());
													EventLoop.Current.Pend(() => callback(text));
												});
		}
		
		public static void ReadAllLines(string path, Action<string[]> callback)
		{
			ReadAllLines(path, Encoding.UTF8, DefaultBufferSize16K, callback);
		}
		public static void ReadAllLines(string path, Encoding encoding, Action<string[]> callback)
		{
			ReadAllLines(path, encoding, DefaultBufferSize16K, callback);
		}
		public static void ReadAllLines(string path, Encoding encoding, int bufferSize, Action<string[]> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			if (encoding == null) throw new ArgumentNullException("encoding");
			FileReadAsync(path, bufferSize, (state) =>
												{
													var lines = new List<string>();
													using (var reader = new StreamReader(state.OutputStream))
													{
														while (reader.Peek() >= 0)
														{
															lines.Add(reader.ReadLine());
														}
													}
													EventLoop.Current.Pend(() => callback(lines.ToArray()));
												});
		}
		static void FileReadAsync(string path, int bufferSize, Action<AsyncFileReadState> complete)
		{
			var fs = System.IO.File.OpenRead(path);
			var state = new AsyncFileReadState(fs, complete, bufferSize);
			fs.BeginRead(state.Buffer, 0, state.BufferSize, FileReadCallback, state);
		}
		static void FileReadCallback(IAsyncResult result)
		{
			var state = (AsyncFileReadState) result.AsyncState;
			var bytesRead = state.FileStream.EndRead(result);
			state.BytesRemaining -= bytesRead;
			if (state.BytesRemaining > 0)
			{
				state.OutputStream.Write(state.Buffer, 0, Math.Min(bytesRead, state.BufferSize));
				state.FileStream.BeginRead(state.Buffer, 0, state.BufferSize, FileReadCallback, state);
			}
			else
			{ 
				//reset the position for use in Completion delegate.
				state.OutputStream.Position = 0;
				state.Complete(state);
				state.Dispose();
			}
		}
	}

	public class AsyncFileReadState : IDisposable
	{
		public readonly int BufferSize;
		public readonly Action<AsyncFileReadState> Complete;
		public readonly FileStream FileStream;
		public readonly MemoryStream OutputStream;
		public byte[] Buffer;
		public long BytesRemaining;

		public AsyncFileReadState(FileStream fs, Action<AsyncFileReadState> complete, int bufferSize)
		{
			if (fs == null) throw new ArgumentNullException("fs");
			if (complete == null) throw new ArgumentNullException("complete");
			if (bufferSize <= 0) throw new ArgumentOutOfRangeException("bufferSize", bufferSize, "Must be a positive integer.");
			FileStream = fs;
			Complete = complete;
			BufferSize = bufferSize;
			OutputStream = new MemoryStream();
			Buffer = new byte[BufferSize];
			BytesRemaining = fs.Length;
		}

		~AsyncFileReadState()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (FileStream != null)
			{
				FileStream.Dispose();
			}
			if (OutputStream != null)
			{
				OutputStream.Dispose();
			}
		}
	}
}
