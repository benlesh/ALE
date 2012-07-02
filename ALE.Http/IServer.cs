using System;
using ALE.Views;

namespace ALE.Http
{
	public interface IServer
	{
		/// <summary>
		///   Starts the server listening an any number of URI prefixes.
		/// </summary>
		/// <param name="prefixes"> URI prefixes for the server to listen on. </param>
		/// <returns> The server it started. </returns>
		IServer Listen(params string[] prefixes);

		/// <summary>
		///   Stop the server.
		/// </summary>
		/// <param name="stopEventLoop"> </param>
		void Stop(bool stopEventLoop = false);

		/// <summary>
		///   Adds a processor to the server processing event.
		/// </summary>
		/// <param name="processor"> The processor to add. </param>
		/// <returns> The server instance. </returns>
		IServer Use(Action<IRequest, IResponse> processor);

	    /// <summary>
	    /// Sets the view processor for the server.
	    /// </summary>
	    /// <param name="viewProcessor">The view processor to use.</param>
	    /// <returns>The server instance.</returns>
	    IServer Use(IViewProcessor viewProcessor);
	}
}