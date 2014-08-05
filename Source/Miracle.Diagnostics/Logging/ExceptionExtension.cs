using System;

namespace Miracle.Diagnostics.Logging
{
    /// <summary>
    /// Exception extension methods
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// Get reference to exception that has no InnterException.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception GetInnermostException(this Exception ex)
        {
            return ex.InnerException != null
                ? GetInnermostException(ex.InnerException)
                : ex;
        }
    }
}