using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Miracle.Diagnostics.Logging;
using Miracle.Macros;

namespace Miracle.Diagnostics.Mail
{
	/// <summary>
	/// XML ILog implementation.
	/// </summary>
	public class MailLog : LogBase
	{
	    /// <summary>
		/// Default constructor.
		/// </summary>
		public MailLog()
	    {
	        Encoding = Encoding.UTF8;
	        AttachmentName = "log.xml";
	    }

	    /// <summary>
		/// Constructor specifying filename of log attachment.
		/// </summary>
		public MailLog(string attachmentName)
            : this()
	    {
	        AttachmentName = attachmentName;
	    }

	    /// <summary>
	    /// Get currently active logfile
	    /// </summary>
	    public Encoding Encoding { get; set; }

	    /// <summary>
	    /// Name of attachment
	    /// </summary>
	    public string AttachmentName { get; set; }

	    /// <summary>
		/// Persist log entry to logfile.
		/// </summary>
		/// <param name="entry">Log entry to add</param>
		public override void AddAlways(ILogEntry entry)
		{
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			var serializer = new XmlSerializer(entry.GetType());
			var memoryStream = new MemoryStream();
			var sw = new StreamWriter(memoryStream, Encoding);
			var xw = new XmlTextWriter(sw) {Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1};

			xw.WriteStartDocument();
			serializer.Serialize(xw, entry, ns);
			xw.Flush();

			memoryStream.Seek(0, SeekOrigin.Begin);

	        var attachmentName = Environment.ExpandEnvironmentVariables(AttachmentName.ExpandMacros(entry));
			var attachment = new Attachment(memoryStream, attachmentName, "text/xml");
			var message = new AdminMailMessage();
			message.Attachments.Add(attachment);

			message.Subject = Environment.ExpandEnvironmentVariables(message.Subject.ExpandMacros(entry));
			message.Body = Environment.ExpandEnvironmentVariables(message.Body.ExpandMacros(entry));

			message.Send();
		}
	}
}