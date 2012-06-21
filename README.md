ALE v 0.0.2.1

Created by Ben Lesh

Licensed under GNU General Public License v 3

===

Another Looping Event architecture

This is something I whipped up for fun, mostly as a proof of concept. I like it though, and I'm looking for feedback.

Hopefully one of you people out there can tell me why what I'm doing is wrong/bad/slow so I can fix it and learn from it.

Thank you for any feedback you might have.

I'm going to continue to work on this as time allows. I'm looking into adding support for non-blocking implementations of WebSockets and Entity Framework extensions for the same.

See my blog at http://www.benlesh.com for more information (posts tagged with ALE)

 * v 0.0.2.2 - Fixed a few issues
    * Fixed an issue where EventLoop wouldn't pause when Events Queue was empty.
	* Added Do.Async functionality.
	* Tested multithreaded event-loop processing.
 * v 0.0.2.1 - Updated Sql client Reader call to .NET's non-blocking async call.
 * v 0.0.2.0 - Updated all I/O called to .NET's non-blocking async calls.
 * v 0.0.1.0 - Added a simple SqlClient implementation, ExecuteReader only for now. More soon.