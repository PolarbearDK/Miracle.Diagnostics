using System;
using System.Configuration;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Logging
{
    /// <summary>
    /// Each log rule is Deserialized from config section to this class.
    /// </summary>
    /// 
    [Serializable]
    [XmlType("rule")]
    public class ConfigSectionRule
    {
        /// <summary>
        /// Is this entry active?
        /// </summary>
        [XmlAttribute("active")]
        public bool Active;

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
        /// String containing Type string to use for logging.
        /// </summary>
        [XmlAttribute("type")]
        public string AdapterType
        {
            get { return _adapterType; }
            set
            {
                _adapterType = value;
                _adapterTypeType = Type.GetType(value);

                if (_adapterTypeType == null)
                    throw new ConfigurationErrorsException("Unable to find logging adapter type:" + AdapterType);

                if (_adapterTypeType.GetInterface("ILog") == null)
                    throw new ConfigurationErrorsException("Logging adapter does not implement ILog interface:" + AdapterType);
            }
        }

        private string _adapterType;

        /// <summary>
        /// Actual type that Type specifies (cached).
        /// </summary>
        private Type _adapterTypeType;

        /// <summary>
        /// Optional parameter for constructing logging object.
        /// </summary>
        [XmlAttribute("param")]
        public string Param;

        /// <summary>
        /// Return boolean indicating if entry us Active
        /// </summary>
        [XmlIgnore]
        public bool IsActive
        {
            get { return Active; }
        }

        /// <summary>
        /// Create instance of log adapter
        /// </summary>
        /// <returns></returns>
        public ILog CreateInstance()
        {
            if (_adapterTypeType != null)
            {
                object[] arguments = Param == null
                                        ? null
                                        : new object[] { Param };

                var instance = (ILog)Activator.CreateInstance(_adapterTypeType, arguments);

                if (instance != null)
                {
                    instance.Level = MinSeveritySpecified ? MinSeverity : SeverityEnum.Debug;
                    return instance;
                }
            }

            throw new ApplicationException("Unable to create instance of log adapter: " + AdapterType);
        }
    }
}