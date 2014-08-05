using System;
using Miracle.Macros;

namespace Miracle.Diagnostics.Logging
{
	/// <summary>
	/// ILog implementation that writes ILogEntry information to console.
	/// </summary>
	public class ConsoleLog : LogBase
	{
		private readonly Macro<ILogEntry> _macro;

	    /// <summary>
		/// Empty constructor. Log file must be opened manually using the open method.
		/// </summary>
		public ConsoleLog()
		{
		}

		/// <summary>
		/// Constructor: Initialize object, and open xml log file.
		/// </summary>
		/// <param name="argument">Logfile argument</param>
		public ConsoleLog(string argument)
		{
			_macro = new Macro<ILogEntry>(Environment.ExpandEnvironmentVariables(argument));
		}

	    /// <summary>
		/// Persist log entry to logfile.
		/// </summary>
		/// <param name="entry">Log entry to add</param>
		public override void AddAlways(ILogEntry entry)
		{
			var backupColor = Console.ForegroundColor;
			try
			{
				// Set console color according to severity
			    Console.ForegroundColor = GetColor(entry.Severity);
				Console.WriteLine(_macro.Expand(entry));
			}
			finally
			{
				Console.ForegroundColor = backupColor;
			}
		}

        /// <summary>
        /// Select console color according to severity
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        private static ConsoleColor GetColor(SeverityEnum severity)
        {
            switch (severity)
            {
                case SeverityEnum.Debug:
                    return ConsoleColor.Gray;
                case SeverityEnum.Information:
                    return ConsoleColor.White;
                case SeverityEnum.Warning:
                    return  ConsoleColor.Yellow;
                case SeverityEnum.Error:
                    return ConsoleColor.Red;
                case SeverityEnum.Fatal:
                    return ConsoleColor.Magenta;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
	}
}