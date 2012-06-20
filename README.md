ALE v 0.0.1.0
===

Another Looping Event architecture

This is something I whipped up for fun, mostly as a proof of concept. I like it though, and I'm looking for feedback.

Hopefully one of you people out there can tell me why what I'm doing is wrong/bad/slow so I can fix it and learn from it.

Thank you for any feedback you might have.

I'm going to continue to work on this as time allows. I'm looking into adding support for non-blocking implementations of WebSockets and Entity Framework extensions for the same.

Here is a sample of how you might use this, a simple web server:

   using ALE.Http;
   Server.Create((request, response) => response.Write("Hello World!")).Listen("127.0.0.1", 1337);
   
Another example for reading a list of files in a directory:

   using ALE.FileSystem;
   Directory.GetFiles(@"C:\Foo", (files) => {
      foreach(var file in files) {
         Console.WriteLine(file);
      }
   });
   
Pretty simple stuff.

v 0.0.1.0 - Added a simple SqlClient implementation, ExecuteReader only for now. More soon.