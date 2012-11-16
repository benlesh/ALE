**ALE - Another Looping Event - (Alpha)**

Created by Ben Lesh
http://www.benlesh.com
ben@benlesh.com

Licensed under MIT license

===

This project is a Node.js style implementation of an event loop architecture in C#. This is something I whipped up for fun, mostly as a proof of concept. I like it though, and I'm looking for feedback.

Thank you for any feedback you might have.

*examples*

To start a webserver:

    EventLoop.Start(() => {
        Server.Create()
		   .Use((req, res) => {
              res.Write("<h1>Hello World</h1>");
           }).Listen("http://*:1337");
    });
    
To set up a web sockets server:

    EventLoop.Start(() => {
        Net.CreateServer((socket) => {
            socket.Receive((text) => {
                socket.Send("Echo: " + text);
            });
        }).Listen("127.0.0.1", 1338, "http://origin.com");
    });
    
Or just to do something like read a file from disk:

    EventLoop.Start(() => {
        File.ReadAllText(@"C:\File.txt", (text) => {
            DoSomething(text);
        });
    });
	
To start a ALE in IIS:

* Start a new web project.
* Reference ALE and ALE.Web.
* Add a reference to the HttpHandler in the Web.Config (here is the minimum Web.config required):

        <?xml version="1.0"?>
        <configuration>
          <system.web>
            <compilation debug="true" targetFramework="4.0" />
         </system.web>
         <system.webServer>
       	 <validation validateIntegratedModeConfiguration="false"/>
         	<modules runAllManagedModulesForAllRequests="true"/>
         	<handlers>
         	  <add verb="*" path="*"
         		name="AleHttpHandler"
         		type="ALE.Web.AleHttpHandler"/>
         	</handlers>
           </system.webServer>
         </configuration>
	 
	 

* Add initialization code to Application_Start in your Global.asax:

    void Application_Start(object sender, EventArgs e)
    {
        // Start the event loop.
        EventLoop.Start();

        // Get the ALE server instance and wire up your middlware.
        ALE.Web.Server.Create()
            .Use((req, res) => res.Write("Hello World"))
            .Use((req, res) => res.Write("<br/>No seriously, I said hello."));
    }

* Add teardown in Application_End in your Global.asax:

    void Application_End(object sender, EventArgs e)
    {
        // Shut down the EventLoop.
        EventLoop.Stop();
    }



See my blog at http://www.benlesh.com for more information (posts tagged with ALE)

Version History
 * v 0.0.10.2 - Added basic Promise implementation
 * v 0.0.9.4 - Updated EventLoops to be slightly more efficient.
   * added unit tests.
   * Fixed issues with Razor templating.
   * Fixed some issues with routing.
 * v 0.0.7.2 - Added FileSystemWatcher implemention.
   * added beginnings of Razor template view processor.
 * v 0.0.7.0 - Added routing.
 * v 0.0.6.1 - Rewrote EventLoop to use Tasks and ContinueWith rather than Actions for performance reasons, and cleaned up the api a little.
   * moved File class to proper namespace.
   * updated File async methods.
   * added File.Read method for streaming file data.
 * v 0.0.5.0 - Added asynchronous http handler for IIS integration.
 * v 0.0.4.6 - Added a static file server implementation.
 * v 0.0.4.4 - Updated web server to use a single event to register all actions
    * removed preprocessor and post processor events.
	* Create method no longer used to register a main event. There is no main event.
 * v 0.0.4.2 - Converted middleware usage to simple event/delegate implementation.
 * v 0.0.4.1 - Abstractions and Middleware capabilities added. Laying the groundwork for a routed server.
    * Abstracted out Server, Request and Response.
	* Added IPreprocessor and IPostprocessor for "before and after" middleware implementations.
	* Added Using method and overloards to IServer to handle attaching middleware.
 * v 0.0.3.0 - Added non-blocking WebSocket implementation
 * v 0.0.2.2 - Fixed a few issues
    * Fixed an issue where EventLoop wouldn't pause when Events Queue was empty.
	* Added Do.Async functionality.
	* Tested multithreaded event-loop processing.
 * v 0.0.2.1 - Updated Sql client Reader call to .NET's non-blocking async call.
 * v 0.0.2.0 - Updated all I/O called to .NET's non-blocking async calls.
 * v 0.0.1.0 - Added a simple SqlClient implementation, ExecuteReader only for now. More soon.