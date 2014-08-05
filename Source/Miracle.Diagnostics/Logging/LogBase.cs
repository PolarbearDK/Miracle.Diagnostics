namespace Miracle.Diagnostics.Logging
{
	/// <summary>
	/// Summary description for LogBase.
	/// </summary>
	public abstract class LogBase : ILog
	{
		private SeverityEnum _level = SeverityEnum.Information;

	    /// <summary>
		/// Set log level filtering. All messages with less severity that log level is discarded.
		/// </summary>
		public SeverityEnum Level
		{
			get { return _level; }
			set { _level = value; }
		}

		/// <summary>
		/// Persist log entry to logfile.
		/// </summary>
		/// <param name="entry">Log entry to add</param>
		public abstract void AddAlways(ILogEntry entry);

		/// <summary>
		/// Persist log entry to logfile if severity is above current severity level.
		/// </summary>
		/// <param name="entry">Log entry to add</param>
		public void Add(ILogEntry entry)
		{
			if (entry.Severity >= _level)
			{
				AddAlways(entry);
			}
		}
	}
}