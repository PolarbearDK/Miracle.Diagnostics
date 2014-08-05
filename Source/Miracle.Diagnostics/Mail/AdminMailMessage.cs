using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using Miracle.Diagnostics.Logging;

namespace Miracle.Diagnostics.Mail
{
	/// <summary>
	/// Specialized MailMessage that obtains all settings from config file.
	/// </summary>
	public class AdminMailMessage : MailMessage
	{
		// Static reference to config section
		private static ConfigSection Config
		{
			get { return (ConfigSection) ConfigurationManager.GetSection("mail"); }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public AdminMailMessage()
		{
			ConfigSection config = Config;

			if (config.Encoding != null)
			{
				Encoding enc = Encoding.GetEncoding(config.Encoding);
				SubjectEncoding = enc;
				BodyEncoding = enc;
			}
			IsBodyHtml = config.IsBodyHtml;

			if (config.From != null) From = config.From;
			if (config.To != null) To.Add(config.To);
			if (config.CC != null) CC.Add(config.CC);
			if (config.Bcc != null) Bcc.Add(config.Bcc);
			if (config.ReplyTo != null) ReplyToList.Add(config.ReplyTo);
			if (config.Subject != null) Subject = config.Subject;
			if (config.Body != null) Body = config.Body;
		}

		/// <summary>
		/// String representation of sender. Is converted to a MailAddress upon Send()
		/// </summary>
		/// <remarks>
		/// This is nessesary because sender can be a macro (fx: ${Person.Email}}, which can't initially be converted to a MailAddress
		/// </remarks>
		public new string From { get; set; }

		/// <summary>
		/// Send mail
		/// </summary>
		public void Send()
		{
			ConfigSection config = Config;

			if (config.IsEnabled)
			{
				var client = new SmtpClient();
				base.From = new MailAddress(From);
				if (config.Server != null) client.Host = config.Server;

				try
				{
					client.Send(this);
					if (config.IsLogging)
					{
						Log.Add(new MailLogEntry(SeverityEnum.Information, "Mail sent", this));
					}
				}
				catch (Exception ex)
				{
				    Log.Add(new MailLogEntry(SeverityEnum.Information, "Failed to send mail", this, ex));
				}
			}
			else
			{
				if (config.IsLogging)
				{
					Log.Add(new MailLogEntry(SeverityEnum.Information, "Mail suppresed by config.", this));
				}
			}
		}
	}
}