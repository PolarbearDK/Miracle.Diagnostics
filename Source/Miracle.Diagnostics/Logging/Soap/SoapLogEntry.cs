using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Logging.Soap
{
//	[Serializable]
//	public class Parameter
//	{
//		/// <summary>
//		/// Name part of parameter
//		/// </summary>
//		[XmlAttribute]
//		public string Name;
//		/// <summary>
//		/// Value of parameter
//		/// </summary>
//		[XmlElement]
//		public object Value;
//	}

	/// <summary>
	/// Implementation of ILogEntry, for persisting a Soap Exception.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "SoapLogEntry")]
	public class SoapLogEntry : ILogEntry
	{
		private DateTime _timestamp = DateTime.Now;
		private SeverityEnum _severity = SeverityEnum.Information;
//		private Parameter[] _parameters = null;

		#region Constructors

		/// <summary>
		/// Constructor required by serialization
		/// </summary>
		public SoapLogEntry()
		{
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="severity">How severe is the entry</param>
		/// <param name="message">Soap message with Exception to be logged</param>
		public SoapLogEntry(SeverityEnum severity, SoapMessage message)
		{
			Exception e = message.Exception.InnerException ?? message.Exception;

			_severity = severity;
			Message = e.Message;
			Exception = e.GetType().ToString();
			Source = e.Source;
			StackTrace = e.StackTrace;
			Action = message.Action;
			Url = message.Url;

//			_parameters = new Parameter[message.MethodInfo.InParameters.Length];
//			for(int i = 0; i < message.MethodInfo.InParameters.Length; i++)
//			{
//				Parameter p = new Parameter();
//				p.Name = message.MethodInfo.InParameters[i].Name;
//				p.Value = message.GetInParameterValue(i);
//				_parameters[i] = p;
//			}
		}

		#endregion

		#region ILogEntry Members

		/// <summary>
		/// Date/Time value for log entry.
		/// </summary>
		public DateTime TimeStamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		/// <summary>
		/// The severity of the entry
		/// </summary>
		public SeverityEnum Severity
		{
			get { return _severity; }
			set { _severity = value; }
		}

		/// <summary>
		/// Log entry message
		/// </summary>
		public string Message { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// Get/Set Exception
		/// </summary>
		public string Exception { get; set; }

		/// <summary>
		/// Get/Set Source
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Get/Set StackTrace 
		/// </summary>
		public string StackTrace { get; set; }

		/// <summary>
		/// Get/Set Action
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// Get/Set Url
		/// </summary>
		public string Url { get; set; }

//		/// <summary>
//		/// Get/Set StackTrace 
//		/// </summary>
//		public Parameter[] Parameters
//		{   
//			get	{	return _parameters;	}
//			set	{ _parameters = value; }
//		}

		#endregion
	}
}