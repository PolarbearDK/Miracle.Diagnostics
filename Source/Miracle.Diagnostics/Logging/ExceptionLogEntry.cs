using System;

namespace Miracle.Diagnostics.Logging
{
    /// <summary>
	/// Implementation of ILogEntry, for persisting an exception.
	/// </summary>
	[Serializable]
	public class ExceptionLogEntry : LogEntry
	{
	    /// <summary>
		/// Default constructor required by serialization
		/// </summary>
		public ExceptionLogEntry()
	    {
	    }

        /// <summary>
        /// Constructor using message and optional exception
        /// </summary>
        /// <param name="severity">How severe is the entry</param>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged (optional)</param>
        public ExceptionLogEntry(SeverityEnum severity, string message, Exception ex)
            : base(severity, message)
        {
            Exception = ExceptionInfo.Factory(ex);
        }

        /// <summary>
		/// Constructor using inner excption message as message
		/// </summary>
		/// <param name="severity">How severe is the entry</param>
		/// <param name="ex">Exception to be logged (required)</param>
		public ExceptionLogEntry(SeverityEnum severity, Exception ex)
            : this(severity, ex.GetInnermostException().Message, ex)
		{
		}

	    /// <summary>
		/// Get/Set Exception
		/// </summary>
		public ExceptionInfo Exception { get; set; }
	}
}