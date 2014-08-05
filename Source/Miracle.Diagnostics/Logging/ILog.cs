using System;

namespace Miracle.Diagnostics.Logging
{
	/// <summary>
	/// How severe is the nature of a log entry?
	/// </summary>
	public enum SeverityEnum
	{
		/// <summary>
		/// Message is only needed for debug purposes.
		/// </summary>
		Debug = 0,
		/// <summary>
		/// Message is informational.
		/// </summary>
		Information = 1,
		/// <summary>
		/// Indicates an non normal situation, that is not an error.
		/// </summary>
		Warning = 2,
		/// <summary>
		/// Indicates an recoverable error.
		/// </summary>
		Error = 3,
		/// <summary>
		/// Highest level of severity. Indicates an unrecoverable error.
		/// </summary>
		Fatal = 4
	}

	/// <summary>
	/// Interface for general log entry.
	/// </summary>
	public interface ILogEntry
	{
		/// <summary>
		/// Date/Time value for log entry.
		/// </summary>
		DateTime TimeStamp { get; set; }

		/// <summary>
		/// The severity of the entry
		/// </summary>
		SeverityEnum Severity { get; set; }

		/// <summary>
		/// Log entry message
		/// </summary>
		string Message { get; set; }
	}

	/// <summary>
	/// Interface for general logging mechanism.
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Severity restriction level. All severity below this level is not logged.
		/// </summary>
		SeverityEnum Level { get; set; }

		/// <summary>
		/// Force a log entry to be logged, even if it doesn't match level restriction.
		/// </summary>
		/// <param name="entry">Log entry to be logged</param>
		void AddAlways(ILogEntry entry);

		/// <summary>
		/// Log entry to be logged, even if it doesn't match level restriction.
		/// </summary>
		/// <param name="entry">Log entry to be logged</param>
		void Add(ILogEntry entry);
	}
}