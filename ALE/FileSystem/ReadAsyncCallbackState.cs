using System;
using System.IO;

namespace ALE.FileSystem
{
	internal class ReadAsyncCallbackState
	{
		public readonly byte[] Buffer;
		public readonly Action<Exception, int, byte[]> Callback;
		public readonly FileStream FileStream;
		public int ReadIndex;

		public ReadAsyncCallbackState(FileStream fs, byte[] buffer, Action<Exception, int, byte[]> callback)
		{
			ReadIndex = 0;
			FileStream = fs;
			Buffer = buffer;
			Callback = callback;
		}
	}
}