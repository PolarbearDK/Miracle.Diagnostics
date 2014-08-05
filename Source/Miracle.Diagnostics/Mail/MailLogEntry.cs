using System;
using System.Net.Mail;
using Miracle.Diagnostics.Logging;

namespace Miracle.Diagnostics.Mail
{
	/// <summary>
	/// Implementation of ILogEntry, for logging e-mail message.
	/// </summary>
	[Serializable]
	public class MailLogEntry : ExceptionLogEntry
	{
        /// <summary>
        /// Default constructor required for serialization
        /// </summary>
        public MailLogEntry()
        {
        }

        /// <summary>
        /// Prefered constructor for exceptions.
        /// </summary>
        /// <param name="severity">Level of severity.</param>
        /// <param name="message">Message to be logged.</param>
        /// <param name="mailMessage">Mail message to be logged.</param>
        /// <returns>An initialized instance of a MailLogEntry.</returns>
        public MailLogEntry(SeverityEnum severity, string message, MailMessage mailMessage)
            : base(severity, message, null)
        {
            From = Format(mailMessage.From);
            To = Format(mailMessage.To);
            Cc = Format(mailMessage.CC);
            Bcc = Format(mailMessage.Bcc);
            ReplyTo = Format(mailMessage.ReplyToList);
            Subject = Format(mailMessage.Subject);
            Body = Format(mailMessage.Body);
        }

        /// <summary>
	    /// Prefered constructor for exceptions.
	    /// </summary>
	    /// <param name="severity">Level of severity.</param>
	    /// <param name="message">Message to be logged.</param>
	    /// <param name="mailMessage">Mail message to be logged.</param>
	    /// <param name="ex">Exception</param>
	    /// <returns>An initialized instance of a MailLogEntry.</returns>
	    public MailLogEntry(SeverityEnum severity, string message, MailMessage mailMessage, Exception ex)
            : base(severity,message,ex)
		{
			From = Format(mailMessage.From);
			To = Format(mailMessage.To);
			Cc = Format(mailMessage.CC);
			Bcc = Format(mailMessage.Bcc);
			ReplyTo = Format(mailMessage.ReplyToList);
			Subject = Format(mailMessage.Subject);
			Body = Format(mailMessage.Body);
		}

	    private static string Format(object o)
	    {
	        if (o == null) return null;
	        var result = o.ToString();
	        return result.Length != 0 ? result : null;
	    }

	    /// <summary>
		/// Get/Set From
		/// </summary>
		public string From { get; set; }

		/// <summary>
		/// Get/Set To
		/// </summary>
		public string To { get; set; }

		/// <summary>
		/// Get/Set CC
		/// </summary>
		public string Cc { get; set; }

		/// <summary>
		/// Get/Set Bcc
		/// </summary>
		public string Bcc { get; set; }

		/// <summary>
		/// Get/Set ReplyTo
		/// </summary>
		public string ReplyTo { get; set; }

		/// <summary>
		/// Get/Set Subject
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Get/Set From
		/// </summary>
		public string Body { get; set; }
	}
}