using System;
using System.IO;

namespace ALE.FileSystem
{
	internal class ReadAsyncCallbackState
	{
		public readonly byte[] Buffer;
		public readonly Action<Exception, long, byte[]> Callback;
		public readonly FileStream FileStream;
	    public long RemainingBytes;

		public ReadAsyncCallbackState(FileStream fs, byte[] buffer, Action<Exception, long, byte[]> callback)
		{
			FileStream = fs;
			Buffer = buffer;
			Callback = callback;
		    RemainingBytes = fs.Length;
		}
	}
}