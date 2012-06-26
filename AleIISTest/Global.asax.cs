using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ALE;

namespace AleIISTest
{
	public class Global : System.Web.HttpApplication
	{

		void Application_Start(object sender, EventArgs e)
		{
			// Start the event loop.
			EventLoop.Start();

			// Get the ALE server instance and wire up your middlware.
			ALE.Web.Server.Create()
				.Use((req, res) => res.Write("Hello World"))
				.Use((req, res) => res.Write("<br/>No seriously, I said hello."));
		}

		void Application_End(object sender, EventArgs e)
		{
			// Shut down the EventLoop.
			EventLoop.Stop();
		}

		void Application_Error(object sender, EventArgs e)
		{
			// Code that runs when an unhandled error occurs

		}

		void Session_Start(object sender, EventArgs e)
		{
			// Code that runs when a new session is started

		}

		void Session_End(object sender, EventArgs e)
		{
			// Code that runs when a session ends. 
			// Note: The Session_End event is raised only when the sessionstate mode
			// is set to InProc in the Web.config file. If session mode is set to StateServer 
			// or SQLServer, the event is not raised.

		}

	}
}
