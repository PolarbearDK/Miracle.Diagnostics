using System;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Logging
{
	/// <summary>
	/// Simple implementation of ILogEntry, for program log.
	/// </summary>
	[Serializable]
	public class LogEntry : ILogEntry
	{
	    private int _code;

	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="severity">Level of severity.</param>
		/// <param name="code">Error code.</param>
		/// <param name="message">Message associated with this log entry.</param>
		/// <param name="class">Logging Class.</param>
		/// <param name="method">Logging Method.</param>
		/// <returns>An initialized instance of a Logentry.</returns>
		public LogEntry(SeverityEnum severity, int code, string message, string @class, string method)
		{
	        TimeStamp = DateTime.Now;
	        Severity = severity;
			_code = code;
			CodeSpecified = true;
			Message = message;
			Class = @class;
			Method = method;
		}

		/// <summary>
		/// Create a log entry.
		/// </summary>
		/// <param name="severity">Level of severity.</param>
		/// <param name="code">Error code.</param>
		/// <param name="message">Message associated with this log entry.</param>
		/// <returns>An initialized instance of a Logentry.</returns>
		public LogEntry(SeverityEnum severity, int code, string message)
		{
		    TimeStamp = DateTime.Now;
		    Severity = severity;
			_code = code;
			CodeSpecified = true;
			Message = message;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="severity">Level of severity</param>
		/// <param name="message">Log message</param>
		public LogEntry(SeverityEnum severity, string message)
		{
		    TimeStamp = DateTime.Now;
		    Severity = severity;
			Message = message;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Log message</param>
		public LogEntry(string message)
		{
		    Severity = SeverityEnum.Information;
		    TimeStamp = DateTime.Now;
		    Message = message;
		}

	    /// <summary>
		/// Constructor
		/// </summary>
		public LogEntry()
	    {
	        Severity = SeverityEnum.Information;
	        TimeStamp = DateTime.Now;
	    }

	    /// <summary>
	    /// Date/Time value for log entry.
	    /// </summary>
	    public DateTime TimeStamp { get; set; }

	    /// <summary>
	    /// The severity of the entry
	    /// </summary>
	    public SeverityEnum Severity { get; set; }

	    /// <summary>
		/// Log entry message
		/// </summary>
		public string Message { get; set; }

	    /// <summary>
	    /// For XML Serializer. Specifies if code has been specified.
	    /// </summary>
	    [XmlIgnore]
	    public bool CodeSpecified { get; set; }

	    /// <summary>
		/// Get/Set error code
		/// </summary>
		public int Code
		{
			get { return _code; }
			set
			{
				_code = value;
				CodeSpecified = true;
			}
		}

		/// <summary>
		/// Get/Set Class name
		/// </summary>
		public string Class { get; set; }

		/// <summary>
		/// Get/Set Method name 
		/// </summary>
		public string Method { get; set; }
	}
}