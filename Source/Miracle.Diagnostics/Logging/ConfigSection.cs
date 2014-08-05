using System;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Logging
{
    /// <summary>
    /// Log config section is Deserialized to this class.
    /// </summary>
    [Serializable]
    [XmlType("log")]
    public class ConfigSection
    {
        /// <summary>
        /// Has MinSeverity been specified?
        /// </summary>
        [XmlIgnore]
        public bool MinSeveritySpecified;

        /// <summary>
        /// Minimum severity that are included in log.
        /// </summary>
        [XmlAttribute("minseverity")]
        public SeverityEnum MinSeverity;

        /// <summary>
        /// Has MaxSeverity been specified?
        /// </summary>
        [XmlIgnore]
        public bool MaxSeveritySpecified;

        /// <summary>
        /// Maximum severity that are included in log.
        /// </summary>
        [XmlAttribute("maxseverity")]
        public SeverityEnum MaxSeverity;

        /// <summary>
        /// List of logging rules.
        /// </summary>
        [XmlElement("rule")]
        public ConfigSectionRule[] Rules;
    }
}