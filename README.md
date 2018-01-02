traceUI is a tool which translates Oracle raw trace file into more readable format. Its primary goal is not to calculate statistics or aggregate the data from the trace file, but just present the course of events in a more readable and convenient format.

It is aimed to be used by developers to search for Oracle errors, unwanted commits or rollbacks, investigate strange application behavior. DBAs also can find it useful.

# What this tool is not
This tool is not a replacement for Oracle tkprof utility or 	
Oracle Trace Analyzer (trcanlzr) or whatever other diagnostic tool. It is supposed to be used in conjunction with other tools.

# Building
* Open the solution in Visual Studio.
* Build the solution.