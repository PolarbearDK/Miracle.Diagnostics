#if !NET40Client 

using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Logging.Web
{
	/// <summary>
	/// Simple key/value class used as an array to represent a name/value collection.
	/// This makes the otherwise unserializable NameValueCollection serializable.
	/// </summary>
	[Serializable]
	public class KeyValue
	{
		/// <summary>
		/// Key or name part of KeyValue
		/// </summary>
		[XmlAttribute] public string Key;

		/// <summary>
		/// Value part of KeyValue
		/// </summary>
		[XmlAttribute] public string Value;
	}

	/// <summary>
	/// Implementation of ILogEntry, for web application log.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "WebLogEntry")]
	public class WebLogEntry : ILogEntry
	{
	    /// <summary>
        /// Default Constructor (required for Serialization)
        /// </summary>
        public WebLogEntry()
        {
            Severity = SeverityEnum.Information;
            TimeStamp = DateTime.Now;
        }

	    /// <summary>
		/// Constructor: Initialize log entry
		/// </summary>
		/// <param name="severity">Severity of entry</param>
		/// <param name="context">Current HTTPContext to build log entry from</param>
		public WebLogEntry(SeverityEnum severity, HttpContext context)
		{
            TimeStamp = DateTime.Now;
            Severity = severity;

            // Log information about last exception
			Exception ex = context.Server.GetLastError();
			if (ex != null)
			{
                Message = ex.GetInnermostException().Message;
                Exception = ExceptionInfo.Factory(ex);
			}

            // Log information about request
			HttpRequest request = context.Request;
			if (request != null)
			{
				ContentEncoding = request.ContentEncoding.EncodingName;
				Form = GetKeyValueArray(request.Form);
				Query = GetKeyValueArray(request.QueryString);
				RequestType = request.RequestType;
				ServerVariables = GetKeyValueArray(request.ServerVariables);
				if (request.RawUrl != null)
				{
					RawUrl = request.RawUrl;
				}
				if (request.Url != null)
				{
					Url = request.Url.ToString();
				}
			}

            // Log information about session
            HttpSessionState session = context.Session;
			if (session != null)
			{
				Session = GetKeyValueArray(session);
			}
		}

	    /// <summary>
	    /// Date/Time value for log entry.
	    /// </summary>
	    public DateTime TimeStamp { get; set; }

	    /// <summary>
	    /// The severity of the entry
	    /// </summary>
	    public SeverityEnum Severity { get; set; }

	    /// <summary>
		/// Log entry message
		/// </summary>
		public string Message { get; set; }

	    /// <summary>
		/// Get/Set request URL
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Get/Set Exception
		/// </summary>
		public ExceptionInfo Exception { get; set; }

		/// <summary>
		/// Get/Set ContentEncoding
		/// </summary>
		public string ContentEncoding { get; set; }

		/// <summary>
		/// Get/Set request RawURL
		/// </summary>
		public string RawUrl { get; set; }

		/// <summary>
		/// Get/Set Request type (GET/POST)
		/// </summary>
		public string RequestType { get; set; }

		/// <summary>
		/// Get/Set Target Site (method)
		/// </summary>
		public string TargetSite { get; set; }

		/// <summary>
		/// Get/Set form collection
		/// </summary>
		[XmlArray(ElementName = "Form")]
		[XmlArrayItem(ElementName = "Item")]
		public KeyValue[] Form { get; set; }

		/// <summary>
		/// Get set query collection
		/// </summary>
		[XmlArray(ElementName = "Query")]
		[XmlArrayItem(ElementName = "Item")]
		public KeyValue[] Query { get; set; }

		/// <summary>
		/// Get/Set session collection
		/// </summary>
		[XmlArray(ElementName = "Session")]
		[XmlArrayItem(ElementName = "Item")]
		public KeyValue[] Session { get; set; }

		/// <summary>
		/// Get set servervariables collection
		/// </summary>
		[XmlArray(ElementName = "ServerVariables")]
		[XmlArrayItem(ElementName = "Item")]
		public KeyValue[] ServerVariables { get; set; }

	    #region Helpers

		private static KeyValue[] GetKeyValueArray(HttpSessionState col)
		{
		    if (col != null && col.Count > 0)
			{
				var kv = new KeyValue[col.Count];
				NameObjectCollectionBase.KeysCollection keys = col.Keys;

				for (int i = 0; i < col.Count; i++)
				{
					kv[i] = new KeyValue {Key = keys[i], Value = col[keys[i]].ToString()};
				}

				return kv;
			}
		    
            return null;
		}

	    private static KeyValue[] GetKeyValueArray(NameValueCollection col)
	    {
	        if (col != null && col.Count > 0)
			{
				var kv = new KeyValue[col.Count];

				for (var i = 0; i < col.Count; i++)
				{
					kv[i] = new KeyValue {Key = col.AllKeys[i], Value = col[i]};
				}

				return kv;
			}
	        
            return null;
	    }

	    #endregion
	}
}

#endif