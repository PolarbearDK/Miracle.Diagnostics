#if !NET40Client 

using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using Miracle.Diagnostics.Config;

namespace Miracle.Diagnostics.Soap
{
	/// <summary>
	/// Config handler for SoapTraceExtension
	/// </summary>
	public class ConfigSectionHandler : GenericConfigSectionHandler<SoapTraceConfigSection>
	{
	}
}

#endif