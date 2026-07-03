# Questions for the self-check:

1. What is logging?
	- Logging is the process of writing application events, messages, and diagnostics so you can monitor behavior, troubleshoot problems, and understand what the app is doing.
2. What information needs to be logged?
	- Log useful events such as important state changes, warnings, errors, unexpected input, and key business actions. Do not log unnecessary noise or sensitive data.
3. What log levels do you know and how do they differ from each other?
	- Common levels are Trace, Debug, Information, Warning, Error, and Critical. Trace and Debug are for detailed diagnostics, Information is for normal app flow, Warning means something unexpected but recoverable happened, Error means a failure occurred, and Critical means a severe failure that may stop the app.
4. What is ILogger in .NET Core?
	- ILogger in .NET Core is the main logging abstraction used by applications. It lets you write logs without depending directly on a specific provider such as Serilog, Console, or Debug.
5. Is it possible to stop logging with Serilog? If "yes", how can you do it?
	- Yes. With Serilog you can stop or reduce logging by changing the minimum level, removing sinks, disabling the logger, or configuring filtering rules. For example, raising the minimum level to `Warning` will suppress `Information` and `Debug` messages.
6. What information is good to include in a log message?
	- A good log message includes what happened, the relevant identifier or context, the log level, and enough detail to diagnose the issue later. It should be clear, short, and actionable.
7. How can Exceptions be logged?
	- Exceptions can be logged by passing the exception object to the logger, for example `logger.LogError(ex, "Something failed");`. This records the stack trace and the error message.
8. What is the default log level?
	- The default minimum log level is usually `Information` in ASP.NET Core applications unless it is changed in configuration.
9. Where can custom logging provider and corresponding logger be registered?
	- A custom logging provider and logger are registered in the application startup pipeline, usually in `Program.cs` when using minimal hosting, or in `Startup.cs` in older ASP.NET Core apps.
