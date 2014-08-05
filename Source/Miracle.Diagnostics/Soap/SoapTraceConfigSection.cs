using System;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Soap
{
	/// <summary>
	/// Configuration section for SoapTraceExtension
	/// </summary>
	[Serializable]
	[XmlType("soaptrace")]
	public class SoapTraceConfigSection
	{
		/// <summary/>
		[XmlAttribute("filename")] public string Filename;
	}
}