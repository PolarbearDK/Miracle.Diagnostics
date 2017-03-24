using System;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Mail
{
	/// <summary>
	/// Log config section is Deserialized to this class.
	/// </summary>
	[Serializable]
	[XmlType("mail")]
	public class ConfigSection
	{
        /// <summary>
        /// Constructor with defaults
        /// </summary>
        public ConfigSection()
        {
            Log = true;
            Enabled = true;
        }

	    /// <summary/>
	    [XmlAttribute("enabled")]
	    public bool Enabled { get; set; }

	    /// <summary/>
		[XmlIgnore]
		public bool EnabledSpecified { get; set; }

	    /// <summary>Log each e-mail transmission in log.</summary>
	    [XmlAttribute("log")]
	    public bool Log { get; set; }

	    /// <summary/>
		[XmlIgnore]
		public bool LogSpecified { get; set; }

		/// <summary>E-mail server</summary>
		[XmlElement("server")]
		public string Server { get; set; }

		/// <summary>Credentials for server</summary>
		[XmlElement("username")]
		public string Username { get; set; }

		/// <summary>Credentials for server</summary>
		[XmlElement("password")]
		public string Password { get; set; }

		/// <summary>Credentials for server</summary>
		[XmlElement("domain")]
		public string Domain { get; set; }

		/// <summary/>
		[XmlElement("encoding")]
		public string Encoding { get; set; }

		/// <summary/>
		[XmlElement("from")]
		public string From { get; set; }

		/// <summary/>
		[XmlElement("to")]
		public string To { get; set; }

		/// <summary/>
		[XmlElement("cc")]
		public string CC { get; set; }

		/// <summary/>
		[XmlElement("bcc")]
		public string Bcc { get; set; }

		/// <summary/>
		[XmlElement("replyto")]
		public string ReplyTo { get; set; }

		/// <summary/>
		[XmlElement("subject")]
		public string Subject { get; set; }

		/// <summary/>
		[XmlElement("body")]
		public string Body { get; set; }

		///<summary/>
		[XmlElement("html")]
		public bool Html { get; set; }

		///<summary/>
		[XmlIgnore]
		public bool HtmlSpecified { get; set; }

		/// <summary>
		/// Is emailing enabled? Default is true.
		/// </summary>
		public bool IsEnabled
		{
			get { return EnabledSpecified == false || Enabled; }
		}

		/// <summary>
		/// use HTML body.
		/// </summary>
		public bool IsBodyHtml
		{
			get { return HtmlSpecified || Html; }
		}

		/// <summary>
		/// Is Loggging enabled? Default is false.
		/// </summary>
		public bool IsLogging
		{
			get { return LogSpecified && Log; }
		}
	}
}