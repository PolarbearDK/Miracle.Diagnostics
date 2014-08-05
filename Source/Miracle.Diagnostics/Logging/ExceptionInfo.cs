using System;
using System.Xml.Serialization;

namespace Miracle.Diagnostics.Logging
{
    /// <summary>
    /// Class that exposes recursive information about exceptions to the serializer.
    /// </summary>
    [Serializable]
    public class ExceptionInfo
    {
        /// <summary>
        /// Factory method. This is the prefered method to initialize this class.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ExceptionInfo Factory(Exception ex)
        {
            if (ex != null)
            {
                return new ExceptionInfo
                {
                    Type = ex.GetType().ToString(),
                    Message = ex.Message,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    TargetSite = String.Format("{0}", ex.TargetSite),
                    InnerException = Factory(ex.InnerException)
                };
            }

            return null;
        }

        /// <summary>Exception type</summary>
        [XmlElement(Order = 1)]
        public string Type { get; set; }

        /// <summary/>
        [XmlElement(Order = 2)]
        public string Message { get; set; }

        /// <summary/>
        [XmlElement(Order = 3)]
        public string Source { get; set; }

        /// <summary/>
        [XmlElement(Order = 4)]
        public string StackTrace { get; set; }

        /// <summary/>
        [XmlElement(Order = 5)]
        public string TargetSite { get; set; }

        /// <summary>Access to inner exception details</summary>
        [XmlElement(Order = 6)]
        public ExceptionInfo InnerException { get; set; }
    }
}