using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Config
{
	/// <summary>
	/// Generic config section handler.
	/// </summary>
	public class GenericConfigSectionHandler<T> : IConfigurationSectionHandler
	{
	    /// <summary>
		/// Implemented by all configuration section handlers to parse the XML of the configuration section. The returned object is added to the configuration collection and is accessed by GetConfig.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A configuration object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			var xmlNodeReader = new XmlNodeReader(section);
			var xmlSerializer = new XmlSerializer(typeof(T));
			xmlSerializer.UnknownAttribute += UnknownAttribute;
			xmlSerializer.UnknownElement += UnknownElement;
			xmlSerializer.UnknownNode += UnknownNode;
			return (T)xmlSerializer.Deserialize(xmlNodeReader);
		}

	    private static void UnknownNode(object sender, XmlNodeEventArgs e)
		{
			throw new ConfigurationErrorsException(string.Format("Unknown node:{0} while deserializing config type {1}", e.Name, typeof (T).FullName));
		}

		private static void UnknownElement(object sender, XmlElementEventArgs e)
		{
			throw new ConfigurationErrorsException(string.Format("Unknown element:{0} while deserializing config type {1}", e.Element.Name, typeof(T).FullName));
		}

		private static void UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			throw new ConfigurationErrorsException(string.Format("Unknown attribute:{0} while deserializing config type {1}", e.Attr.Name, typeof(T).FullName));
		}
	}
}