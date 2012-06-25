using System.Security.Principal;
using System.Collections.Generic;
using System;

namespace ALE.Http
{
    public interface IContext
    {
        /// <summary>
        /// A data store to be leveraged by middleware for 
        /// passing values between different stages of processing.
        /// </summary>
        dynamic ContextBag { get; set; }
        /// <summary>
        /// The response object.
        /// </summary>
        IResponse Response { get; }
        /// <summary>
        /// The requrest object.
        /// </summary>
        IRequest Request { get; }
        /// <summary>
        /// The user security information.
        /// </summary>
        IPrincipal User { get; }
        /// <summary>
        /// If set true will halt the execution of
        /// remaining middleware.
        /// </summary>
        bool IsExecutionComplete { get; }
        /// <summary>
        /// When called, will halt the execution 
        /// of remaining middleware
        /// </summary>
        void Complete();
    }
}