Miracle.Diagnostics
===================

Simple logging framework.

- Logs to always valid XML log file. 
- Is able to handle extremely large log files (1Tb is no problem). 
- Exceptions are logged with all available information including inner exceptions.
- Unobtrusive installation in existing ASP.NET application to log all errors. 

NuGet package includes config settings for console and ASP.NET.

Log directly from code:
````
Log.Debug("Hello world");
Log.Information("Hello world {0}","using composite formatting");
Log.Warning("Hello world warning");
Log.Error("Hello world error");
Log.Fatal("Hello world fatal");
````
Log applications errors with all exception information:
````
Log.Error(ex);
or
Log.Error(ex, "My custom message");
````

Log custom information tailored directly to your application (implement ILogEntry, or inherit from existing LogEntry classes):
````
Log.Add(new ExceptionLogEntry(SeverityEnum.Error, ex));
Log.Add(new AuditLogEntry(SeverityEnum.Information, userName, area));
````
