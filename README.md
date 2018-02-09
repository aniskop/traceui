# traceUI
traceUI (pronounced /'treɪs ju: aɪ/) is a command-line tool which translates Oracle raw trace file into more readable format. Its primary goal is not to calculate statistics or aggregate the data from the trace file, but just present the course of events in a more readable and convenient format.

It is aimed to be used by developers to search for Oracle errors, unwanted commits or rollbacks, investigate strange application behavior. DBAs also can find it useful.

# What this tool is not
This tool is not a replacement for Oracle tkprof utility or 	
Oracle Trace Analyzer (trcanlzr) or whatever other diagnostic tool. It is supposed to be used in conjunction with other tools.

# Building
* Open the solution in Visual Studio.
* Build the solution.

# Usage
Syntax:
```
traceuic [options] input_file_name [output_file_name]
```
To see program help and all options:
```
traceuic
```
Translate Oracle trace using default options (skip system queries, skip waits) and autogenerated output file name mytrace.report.txt:
```
traceuic c:\temp\mytrace.trc
```
Translate Oracle trace using default options (skip system queries, skip waits) to specified output file:
```
traceuic c:\temp\mytrace.trc c:\temp\mytrace.txt
```

# Known issues
1. Bind variable might not be parsed and displayed correctly when bind entry contains something like memory dump. Then binds from different statement along with `EXEC` and other text might be displayed.
2. Time offset from the start of the trace file might be displayed incorrectly in some cases (situation not reproduced for now). It seems the reason is when under some circumstances `tim` property of the entry is less than the `tim` of the previous one, but should be greater.
