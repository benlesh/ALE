**ALE - Another Looping Event - (Alpha)**

Created by Ben Lesh

Licensed under GNU General Public License v 3

===

This project is a Node.js style implementation of an event loop architecture in C#. This is something I whipped up for fun, mostly as a proof of concept. I like it though, and I'm looking for feedback.

Thank you for any feedback you might have.

*examples*

To start a webserver:

    EventLoop.Start(() => {
        Server.Create((req, res) => {
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
    
Within the EventLoop.Start callback any number of the above could occur. This is more than just a web server, it's an event loop architecture.

See my blog at http://www.benlesh.com for more information (posts tagged with ALE)

 * v 0.0.3.0 - Added non-blocking WebSocket implementation
 * v 0.0.2.2 - Fixed a few issues
    * Fixed an issue where EventLoop wouldn't pause when Events Queue was empty.
	* Added Do.Async functionality.
	* Tested multithreaded event-loop processing.
 * v 0.0.2.1 - Updated Sql client Reader call to .NET's non-blocking async call.
 * v 0.0.2.0 - Updated all I/O called to .NET's non-blocking async calls.
 * v 0.0.1.0 - Added a simple SqlClient implementation, ExecuteReader only for now. More soon.