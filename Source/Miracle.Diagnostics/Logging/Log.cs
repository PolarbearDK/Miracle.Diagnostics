using System;
using System.Configuration;

namespace Miracle.Diagnostics.Logging
{
	/// <summary>
	/// Static class for logging through logging framework.
	/// </summary>
	public class Log
	{
		private static ConfigSection Config
		{
			get { return (ConfigSection) ConfigurationManager.GetSection("log"); }
		}

		/// <summary>
		/// Get/Set object used to get context from.
		/// </summary>
		public static object Context { get; set; }

		/// <summary>
		/// Add log entry to global logging infrastructure
		/// </summary>
		/// <param name="entry"></param>
		public static void Add(ILogEntry entry)
		{
			if (
				(Config.MinSeveritySpecified == false || entry.Severity >= Config.MinSeverity) &&
				(Config.MaxSeveritySpecified == false || entry.Severity <= Config.MaxSeverity)
				)
			{
				foreach (var rule in Config.Rules)
				{
					if (rule.IsActive)
					{
						if (
							(rule.MinSeveritySpecified == false || entry.Severity >= rule.MinSeverity) &&
							(rule.MaxSeveritySpecified == false || entry.Severity <= rule.MaxSeverity)
							)
						{
							ILog log = rule.CreateInstance();
							log.Add(entry);
						}
					}
				}
			}
		}

		#region Debug helpers

		/// <summary>
		/// Log message using Debug severity
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Debug(string message)
		{
			Add(new LogEntry(SeverityEnum.Debug, message));
		}

		/// <summary>
		/// Log formatted message using Debug severity
		/// </summary>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Debug(string messageFormat, params object[] args)
		{
			Debug(string.Format(messageFormat, args));
		}

		#endregion

		#region Information helpers

		/// <summary>
		/// Log message using Information severity
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Information(string message)
		{
			Add(new LogEntry(SeverityEnum.Information, message));
		}

		/// <summary>
		/// Log formatted message using Information severity
		/// </summary>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Information(string messageFormat, params object[] args)
		{
			Information(string.Format(messageFormat, args));
		}

		#endregion

		#region Warning helpers

		/// <summary>
		/// Log message using Warning severity
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Warning(string message)
		{
			Add(new LogEntry(SeverityEnum.Warning, message));
		}

		/// <summary>
		/// Log formatted message using Warning severity
		/// </summary>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Warning(string messageFormat, params object[] args)
		{
			Warning(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Log exception using Warning severity
		/// </summary>
		/// <param name="ex">Exception to log</param>
		public static void Warning(Exception ex)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Warning, ex));
		}

		/// <summary>
		/// Log exception using Warning severity
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="ex">Exception to log</param>
		public static void Warning(Exception ex, string message)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Warning, message, ex));
		}

		/// <summary>
		/// Log exception using Warning severity
		/// </summary>
		/// <param name="ex">Exception to log</param>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Warning(Exception ex, string messageFormat, params object[] args)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Warning, string.Format(messageFormat, args), ex));
		}

		#endregion

		#region Error helpers

		/// <summary>
		/// Log message using Error severity
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Error(string message)
		{
			Add(new LogEntry(SeverityEnum.Error, message));
		}

		/// <summary>
		/// Log formatted message using Error severity
		/// </summary>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Error(string messageFormat, params object[] args)
		{
			Error(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Log exception using Error severity
		/// </summary>
		/// <param name="ex">Exception to log</param>
		public static void Error(Exception ex)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Error, ex));
		}

		/// <summary>
		/// Log exception using Error severity
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="ex">Exception to log</param>
		public static void Error(Exception ex, string message)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Error, message, ex));
		}

		/// <summary>
		/// Log exception using Error severity
		/// </summary>
		/// <param name="ex">Exception to log</param>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Error(Exception ex, string messageFormat, params object[] args)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Error, string.Format(messageFormat, args), ex));
		}

		#endregion

		#region Fatal helpers

		/// <summary>
		/// Log message using Fatal severity
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Fatal(string message)
		{
			Add(new LogEntry(SeverityEnum.Fatal, message));
		}

		/// <summary>
		/// Log formatted message using Fatal severity
		/// </summary>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Fatal(string messageFormat, params object[] args)
		{
			Fatal(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Log exception using Fatal severity
		/// </summary>
		/// <param name="ex">Exception to log</param>
		public static void Fatal(Exception ex)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Fatal, ex));
		}

		/// <summary>
		/// Log exception using Fatal severity
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="ex">Exception to log</param>
		public static void Fatal(Exception ex, string message)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Fatal, message, ex));
		}

		/// <summary>
		/// Log exception using Fatal severity
		/// </summary>
		/// <param name="ex">Exception to log</param>
		/// <param name="messageFormat">Message format to log</param>
		/// <param name="args">Argument for message format</param>
		public static void Fatal(Exception ex, string messageFormat, params object[] args)
		{
			Add(new ExceptionLogEntry(SeverityEnum.Fatal, string.Format(messageFormat, args), ex));
		}

		#endregion
	}
}