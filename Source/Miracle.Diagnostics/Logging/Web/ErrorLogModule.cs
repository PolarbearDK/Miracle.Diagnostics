#if !NET40Client 

using System;
using System.Web;

namespace Miracle.Diagnostics.Logging.Web
{
	/// <summary>
	/// HttpModule for automatic error logging from web.config.
	/// </summary>
	public class ErrorLogModule : IHttpModule
	{
	    /// <summary>
		/// Initialize module
		/// </summary>
		/// <param name="context">Application context</param>
		public void Init(HttpApplication context)
		{
			context.Error += Error;
		}

		/// <summary>
		/// Dispose object.
		/// </summary>
		public void Dispose()
		{
			// Nothing to dispose.
		}

	    private void Error(Object sender, EventArgs e)
		{
			Exception exc = HttpContext.Current.Server.GetLastError();

			// Do not log errors below 500 (like 404: not found).
			if (exc is HttpException && ((HttpException) exc).GetHttpCode() < 500) return;

			// Add new WebLogEntry to logging framework
			Log.Add(new WebLogEntry(SeverityEnum.Error, HttpContext.Current));
		}
	}
}

#endif