# ProcMonX

## Extended Process Monitor-like tool based on Event Tracing for Windows

The classic Sysinternals tool [Process Monitor](https://docs.microsoft.com/en-us/sysinternals/downloads/procmon) uses a file system minifilter, registry minifilter and process/thread callbacks to get the information it provides.

An alternative way is to use Event Tracing for Windows (ETW) to get this information, without the need for a kernel driver. (Process Monitor does use ETW for network events).

See more info at [this blog post](http://blogs.microsoft.co.il/pavely/2018/01/17/procmon-vs-procmonx/).

![ProcMonX](https://github.com/zodiacon/ProcMonX/blob/master/procmonx1.PNG)
