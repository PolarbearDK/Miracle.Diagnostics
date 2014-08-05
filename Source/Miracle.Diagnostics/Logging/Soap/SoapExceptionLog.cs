using System;
using System.Web.Services.Protocols;

namespace Miracle.Diagnostics.Logging.Soap
{
	/// <summary>
	/// Summary description for SoapExceptionLog.
	/// </summary>
	public class SoapExceptionLogExtension : SoapExtension
	{
		/// <summary>
		/// Handles initialization when the SOAP extension is applied using an SoapExtensionAttribute
		/// </summary>
		/// <param name="methodInfo">The method that the SoapExtensionAttribute is declared on</param>
		/// <param name="attribute">The declared SoapExtensionAttribute containing the filename</param>
		/// <returns>The filename contanined in the SoapExtensionAttribute</returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			//return ((SoapExceptionLogExtensionAttribute) attribute).Filename;
			return null;
		}

		// The SOAP extension was configured to run using a configuration file
		// instead of an attribute applied to a specific XML Web service
		// method.
		/// <summary>
		/// Handles initialization when the SOAP extension is applied using the app.config file.
		/// </summary>
		/// <param name="WebServiceType">The Type of the WebService being called</param>
		/// <returns>
		///		A filename consisting of the first logical drive on the local maching, concatenated with
		///		the name of the Web Service, e.g. "c:\HelloWorld.xml"
		/// </returns>
		public override object GetInitializer(Type WebServiceType)
		{
			//return Environment.GetLogicalDrives()[0] + "\\" + WebServiceType.FullName + ".xml";    
			return null;
		}

		/// <summary>
		/// Performs the actual initialization, in this case storing the filename in a member variable
		/// </summary>
		/// <param name="initializer">The initializer object returned by either 
		///		GetInitializer(Type WebServiceType) or 
		///		GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		///</param>
		public override void Initialize(object initializer)
		{
			//filename = (string) initializer;
		}

		//  If the SoapMessageStage is such that the SoapRequest or
		//  SoapResponse is still in the SOAP format to be sent or received,
		//  save it out to a file.
		/// <summary>
		/// Processes the SoapMessage. If the SoapMessageStage is AfterSerialize, the message is assumed to
		/// be a SoapRequest (since this SoapExtension must be applied to the client),
		/// and if the SoapMessageStage is BeforeDeserialize, the SoapMessage is assumed to be a SoapResponse.
		/// In either case, the SoapMessage is written to the XML log file.
		/// </summary>
		/// <param name="message">The SoapMessage to process</param>
		public override void ProcessMessage(SoapMessage message)
		{
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeDeserialize:
					//WriteInput(message);
					//WriteXmlInput(message, "SoapResponse");
					break;
				case SoapMessageStage.AfterDeserialize:
					break;
				case SoapMessageStage.BeforeSerialize:
					if (message.Exception != null)
					{
						Log.Add(new SoapLogEntry(SeverityEnum.Error, message));
					}
					break;
				case SoapMessageStage.AfterSerialize:
					//WriteOutput(message);
					//WriteXmlOutput(message, "SoapRequest");
					break;
				default:
					throw new Exception("invalid stage");
			}
		}
	}

	/// <summary>
	/// Create a SoapExtensionAttribute for the SOAP Extension that can be
	/// applied to an XML Web service method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class SoapExceptionLogExtensionAttribute : SoapExtensionAttribute
	{
		/// <summary>
		/// Get type of extension to use for this XML Web service method
		/// </summary>
		public override Type ExtensionType
		{
			get { return typeof (SoapExceptionLogExtension); }
		}

		/// <summary>
		/// Get/Set priority of extension
		/// </summary>
		public override int Priority { get; set; }
	}
}