#Cross-Platform Peer-To-Peer Library Using Telehash
This is an attempt at an implementation of the telehash protocol with maximal source sharing across Windows, Linux, OSX, iOS, Android and Windows Phone (see below). I've chosen to use C# for at least 2 reasons:

* There is a lot of enterprise support for it (.NET, Mono + touch + droid) and you can do most everything from within Visual Studio
* There isn't an ongoing version of telehash using it

Eventually I plan to merge this with the official telehash project but at current this source is like baby's first sockets application :) I've tried to build up the structure of the code strictly following the telehash conventions so hopefully its readable.

###Tools Used
* Visual Studio 2010 (for Windows development)
* [Project Linker](http://visualstudiogallery.msdn.microsoft.com/5e730577-d11c-4f2e-8e2b-cbb87f76c044) for Visual Studio 
* MonoDevelop (for iOS building and debugging)
* Android and iOS SDK's
* Windows Phone 7 SDK
* Xamarin's MonoTouch and Mono for Android
* [JSON.net](http://json.codeplex.com/releases/view/94220) and accompanying portable builds
* [MonoFlave](https://github.com/jamiebriant/VsMono) to compile the iOS MonoTouch source inside VS2010 (helps if you use, for example, ReSharper)
* [MonoTools](http://mono-tools.com/download/) for building and debugging on Linux and OSX from within VS2010
* (Optional) Wireshark for debugging


###To Dos
* Test on Linux, Android and iOS
* Finish the Telex object to parse out received JSON and to allow Commands, Headers and Signals to be updated
* Implement various switches corresponding to their function within the network
* Handle special switch functions
* Start the DHT implementation
* Unit tests
* How to traverse other network topologies?
* Documentation: references to protocol, methodology for common source, etc


###Right Nows
* The solution should build out of the chute, but building the Windows Phone 7 project will fail (see below). I have only mildly worked with the Windows build (the "Playground" project) so the actual functionality in Android and iOS is unknown. It only works over a LAN with 2 endpoints on the same subnet (make sure to disable Windows Firewall!).


###Notes
I haven't figured out a good way to implement any type of P2P like protocol for Windows Phone 7. The lack of support for anything but <code>RecieveFromAsync</code> nixes a simple implementation. Hopefully WP8 will have some better networking capabilities.