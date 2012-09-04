# **smokesignal.net** #

The original (.Net) version of the SmokeSignal service (campfire addon)

Visit [www.smokesignalnow.com](http://www.smokesignalnow.com)


# **Overview of the Add-on** #
- Built using Visual Studio 2010, targets on .NET 4
- SmokeSignal is installed on a single host (Windows 7+)
- It is configured to access your desired campfire, 
  e.g. https://myname.campfirenow.com
- It is also configured to use a mail provider
- The install has 2 main components, the Windows "Service" and the
  configuration application (called SmokeSignal.exe). 

That's it. From here on out smokesignal keeps itself up to date. It automatically
detects new rooms and users and listens to smokesignals and sends them out as
needed.



# **Overview of the code** #
The solution file is "smokesignal.sln" found in the same directory as this ReadMe. There are 6 associated VS projects:

- **Test_SmokeSignal**
   Contains unit tests for a good part of the smoke signal functionlaity
- **campfire.NET**
   A little .NET wrapper library around the campfireAPI functionlaity needed 
   by smokesignal
- **ServiceInstaller**
   VS configuration into for building the installer (SSInstaller.msi file)
- **SmokeSignal**
   This is the "main()" project of the window's service.
- **NotifierLibrary**
   This is the main guts of the window's service. This is where the action
   happens, the calling of the campfire APIs to implement Smoke Signal 
   functionality
- **SmokeSignalSetup**
   This project defines the standalone configuration application. SmokeSignal
   must be configured using this app before it will work.

# **ToDo's** #
- A big item is getting installation correct. Currently elevated privileges are needed to both
  run the installer and to run the "SmokeSignal.exe" setup application (because it is modifying config
  files owned by the service). This is obviously lame
- Updates. I did a bit of legwork in terms of updates. But more is needed.


# **License** #
Copyright (c) 2012 Peter Potrebic. All rights reserved.

The MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE
