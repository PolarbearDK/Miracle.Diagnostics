using System;
using System.Configuration;
using System.IO;
using System.Web.Services.Protocols;

namespace Miracle.Diagnostics.Soap
{
	/// <summary>
	/// SOAP Extension that traces the SOAP request and SOAP response for the Web service method the SOAP extension is applied to.
	/// </summary>
	public class SoapTraceExtension : SoapExtension
	{
		private Stream originalStream;
		private Stream bufferedStream;
		private XmlTraceFile traceFile;

		/// <summary>
		/// Static reference to Config section
		/// </summary>
		private static SoapTraceConfigSection Config
		{
			get { return (SoapTraceConfigSection) ConfigurationManager.GetSection("soaptrace"); }
		}

		/// <summary>
		/// Save the Stream representing the SOAP request or SOAP response into a local memory buffer.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public override Stream ChainStream(Stream stream)
		{
			originalStream = stream;
			bufferedStream = new MemoryStream();
			return bufferedStream;
		}

		/// <summary>
		/// When the SOAP extension is accessed for the first time, the XML Web service method it is applied to is accessed to store the file name passed in, 
		/// using the corresponding SoapExtensionAttribute.   
		/// </summary>
		/// <param name="methodInfo">Not used</param>
		/// <param name="attribute">Used to get filename from</param>
		/// <returns></returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return String.Format(((TraceExtensionAttribute) attribute).Filename);
		}

		/// <summary>
		/// The SOAP extension was configured to run using a configuration file instead of an attribute applied to a specific Web service method. 
		/// </summary>
		/// <param name="webServiceType"></param>
		/// <returns></returns>
		public override object GetInitializer(Type webServiceType)
		{
			string filename = Config.Filename;

			// Extract format specification for format 0
			string fmt0 = null;
			int fmt0pos = filename.IndexOf("{0");
			if (fmt0pos > 0)
				fmt0 = filename.Substring(fmt0pos, filename.IndexOf('}', fmt0pos) - fmt0pos + 1);

			filename = String.Format(filename, fmt0, webServiceType.Name);
			return filename;
		}

		/// <summary>
		/// Receive the file name stored by GetInitializer and create trace file from it 
		/// </summary>
		/// <param name="initializer"></param>
		public override void Initialize(object initializer)
		{
			traceFile = new XmlTraceFile((string) initializer);
		}


		/// <summary>
		/// If the SoapMessageStage is such that the SoapRequest or SoapResponse is still in the SOAP format to be sent or received, save it out to a file.
		/// </summary>
		/// <param name="message">SoapMessage</param>
		public override void ProcessMessage(SoapMessage message)
		{
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeSerialize:
					break;
				case SoapMessageStage.AfterSerialize:
					WriteOutput(message);
					break;
				case SoapMessageStage.BeforeDeserialize:
					WriteInput(message);
					break;
				case SoapMessageStage.AfterDeserialize:
					break;
				default:
					throw new Exception("invalid stage");
			}
		}

		/// <summary>
		/// Write outgoing message to log.
		/// </summary>
		/// <param name="message"></param>
		private void WriteOutput(SoapMessage message)
		{
			string soapMessageType = (message is SoapServerMessage) ? "SoapResponse" : "SoapRequest";

			traceFile.Add(message.Action, soapMessageType, bufferedStream);

			Copy(bufferedStream, originalStream);
		}

		/// <summary>
		/// Write incomming message to log.
		/// </summary>
		/// <param name="message"></param>
		private void WriteInput(SoapMessage message)
		{
			string soapMessageType = (message is SoapServerMessage) ? "SoapRequest" : "SoapResponse";

			Copy(originalStream, bufferedStream);

			traceFile.Add(message.Action, soapMessageType, bufferedStream);
		}

		/// <summary>
		/// Copy stream
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		private static void Copy(Stream from, Stream to)
		{
			TextReader reader = new StreamReader(from);
			TextWriter writer = new StreamWriter(to);
			writer.WriteLine(reader.ReadToEnd());
			writer.Flush();
		}
	}

	/// <summary>
	/// SoapExtensionAttribute for the SOAP Extension that can be applied to a Web service method. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TraceExtensionAttribute : SoapExtensionAttribute
	{
		private string filename = "c:\\soaplog.txt";

		/// <summary>
		/// Gets the System.Type of the SOAP extension.
		/// </summary>
		public override Type ExtensionType
		{
			get { return typeof (SoapTraceExtension); }
		}

		/// <summary>
		/// Gets or set the priority of the SOAP extension.
		/// </summary>
		public override int Priority { get; set; }

		/// <summary>
		/// Gets or sets the filename used to log messages by this SOAP extension.
		/// </summary>
		public string Filename
		{
			get { return filename; }
			set { filename = value; }
		}
	}
}