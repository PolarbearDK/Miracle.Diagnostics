using System;
using System.IO;
using System.Text;
using System.Threading;
#if !NET40Client 
using System.Web;
#endif
using System.Xml;
using System.Xml.Serialization;
using Miracle.Macros;

namespace Miracle.Diagnostics.Logging
{
	/// <summary>
	/// XML ILog implementation.
	/// </summary>
	public class XmlLogFile : LogBase
	{
		private string _logFileName;
		private Encoding _encoding = Encoding.UTF8;
		private string _rootElement = "Log";
#if !NET40Client 
		private static HttpServerUtility _server;
#endif

	    /// <summary>
		/// Empty constructor. Log file must be opened manually using the open method.
		/// </summary>
		public XmlLogFile()
		{
		}

		/// <summary>
		/// Constructor: Initialize object, and open xml log file.
		/// </summary>
		/// <param name="fileName">Logfile filename to open</param>
		public XmlLogFile(string fileName)
		{
			LogFileName = Environment.ExpandEnvironmentVariables(fileName);
		}

		/// <summary>
		/// Constructor: Initialize object, and open xml log file.
		/// </summary>
		/// <param name="fileName">Logfile filename to open</param>
		/// <param name="encoding">File encoding</param>
		public XmlLogFile(string fileName, Encoding encoding)
		{
			LogFileName = fileName;
			_encoding = encoding;
		}

	    /// <summary>
		/// Position file pointer on end root tag
		/// </summary>
		/// <param name="stream">Open log file stream</param>
		private static void FindEndRootTag(Stream stream)
		{
			var position = stream.Length - 3;

			// Find "/" part of end root tag
			while (position >= 0)
			{
				stream.Seek(position, SeekOrigin.Begin);
				int b = stream.ReadByte();
				if (b == '/') break;
				position--;
			}

			// Find "<" part of end root tag
			while (position >= 0)
			{
				stream.Seek(position, SeekOrigin.Begin);
				int b = stream.ReadByte();
				if (b == '<') break;
				position--;
			}

			// Find ">" of previous tag
			while (position >= 0)
			{
				stream.Seek(position, SeekOrigin.Begin);
				int b = stream.ReadByte();
				if (b == '>') break;
				position--;
			}

			if (position > 0)
				stream.Seek(position, SeekOrigin.Begin);
		}

	    /// <summary>
		/// Get/Set currently active logfile
		/// </summary>
		public string LogFileName
		{
			get { return _logFileName; }
			set
			{
				if (string.IsNullOrEmpty(value)) throw new ApplicationException("Invalid filename.");

#if !NET40Client 
				int splitPoint = value.LastIndexOf('/');

				if (HttpContext.Current != null)
					_server = HttpContext.Current.Server;

				if (splitPoint > 0 && _server != null) //web and something to map
				{
					try
					{
						// split filename on /
						string[] fileParts =
							{
								value.Substring(0, splitPoint),
								value.Substring(splitPoint + 1)
							};

						// Convert first part of filename from virtual to physical file path.
						fileParts[0] = _server.MapPath(fileParts[0]);

						// join parts.
						_logFileName = String.Join(@"\", fileParts);
					}
					catch (HttpException)
					{
						// HttpException is thrown if MapPath is attemtped on a physical filename. 
						// Just ignore it, and save value unchanged.
						_logFileName = value;
					}
				}
				else
#endif
					_logFileName = value;
			}
		}

		/// <summary>
		/// Expand current log file name: Parameter {0} can be specified and is recalculated on each log add.
		/// Alternatively, specify a Context object, and macros in filename will be expanded.
		/// </summary>
		protected string ExpandedLogFileName
		{
			get
			{
				object context = Log.Context;
				if (context != null)
				{
					return _logFileName.ExpandMacros(context);
				}
			    
                // Reformat {0} using current DateTime
			    return String.Format(_logFileName, DateTime.Now);
			}
		}

		/// <summary>
		/// Get currently active logfile
		/// </summary>
		public Encoding LogFileEncoding
		{
			get { return _encoding; }
			set { _encoding = value; }
		}

		/// <summary>
		/// Name of root element in log file.
		/// </summary>
		public string Root
		{
			get { return _rootElement; }
			set
			{
				if (string.IsNullOrEmpty(value)) throw new ApplicationException("Invalid root value.");
				_rootElement = value;
			}
		}

	    /// <summary>
		/// Open log file for exclusive write access. 
		/// If log file is busy, retry after a short delay.
		/// </summary>
		/// <returns>Open file stream</returns>
		protected FileStream OpenFileStream()
		{
			var attempt = 1;
			var fileName = ExpandedLogFileName;
			while (true)
			{
				try
				{
				    return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
				}
				catch (IOException)
				{
					// File is busy
					if (attempt < 10)
					{
						// 1/10 second delay.
						Thread.Sleep(100);
						attempt++;
					}
					else
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Persist log entry to logfile.
		/// </summary>
		/// <param name="entry">Log entry to add</param>
		public override void AddAlways(ILogEntry entry)
		{
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			var serializer = new XmlSerializer(entry.GetType());

		    using (var fs = OpenFileStream())
		    {
		        var isEmpty = (fs.Length == 0);

		        if (!isEmpty) FindEndRootTag(fs);

		        using (var sw = new StreamWriter(fs, _encoding))
		        {
		            using (var xw = new XmlTextWriter(sw) {Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1})
		            {
		                if (isEmpty)
		                {
		                    xw.WriteStartDocument();
		                    xw.WriteStartElement(_rootElement);
		                }
		                else
		                {
		                    // Fool writer to think it is inside root node
		                    long position = fs.Position;
		                    xw.WriteStartElement(_rootElement);
		                    xw.Flush();
		                    fs.Seek(position, SeekOrigin.Begin);
		                }

		                serializer.Serialize(xw, entry, ns);
		                xw.Close();
		            }
		        }
		    }
		}
	}
}