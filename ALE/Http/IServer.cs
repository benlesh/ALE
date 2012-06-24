namespace ALE.Http
{
    public interface IServer
    {
        /// <summary>
        /// Starts the server listening an any number of
        /// URI prefixes.
        /// </summary>
        /// <param name="prefixes">URI prefixes for the server to listen on.</param>
        /// <returns>The server it started.</returns>
        IServer Listen(params string[] prefixes);

        /// <summary>
        /// Stop the server.
        /// </summary>
        /// <param name="stopEventLoop"></param>
        void Stop(bool stopEventLoop = false);

        /// <summary>
        /// Adds preprocessing middleware.
        /// </summary>
        /// <param name="middleware">The middleware to add.</param>
        /// <returns>The server instance.</returns>
        IServer Use(IPreprocessor middleware);

        /// <summary>
        /// Adds postprocessing middleware.
        /// </summary>
        /// <param name="middleware">The middleware to add.</param>
        /// <returns>The server instance.</returns>
        IServer Use(IPostprocessor middleware);
    }
}