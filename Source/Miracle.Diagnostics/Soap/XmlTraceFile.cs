#if !NET40Client 

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Miracle.Diagnostics.Soap
{
	/// <summary>
	/// XML ILog implementation.
	/// </summary>
	public class XmlTraceFile
	{
		private string _fileName;
		private Encoding _encoding = Encoding.UTF8;
		private string _rootElement = "SoapTrace";
		private string _nodeElement = "Trace";

		#region Constructors

		/// <summary>
		/// Constructor: Initialize object, and open xml log file.
		/// </summary>
		/// <param name="FileName">Logfile filename to open</param>
		public XmlTraceFile(string FileName)
		{
			LogFileName = FileName;
		}

		/// <summary>
		/// Constructor: Initialize object, and open xml log file.
		/// </summary>
		/// <param name="FileName">Logfile filename to open</param>
		/// <param name="encoding">File encoding</param>
		public XmlTraceFile(string FileName, Encoding encoding)
		{
			LogFileName = FileName;
			_encoding = encoding;
		}

		#endregion

		#region Internal Helper functions

		/// <summary>
		/// Open log file for exclusive write access. 
		/// If log file is busy, retry after a short delay.
		/// </summary>
		/// <returns>Open file stream</returns>
		protected FileStream OpenFileStream()
		{
			int Attempt = 1;
			string FileName = ExpandedLogFileName;
			while (true)
			{
				try
				{
					var fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
					return fs;
				}
				catch (IOException)
				{
					// File is busy
					if (Attempt < 10)
					{
						// 1/10 second delay.
						Thread.Sleep(100);
						Attempt++;
					}
					else
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Position file pointer on end root tag
		/// </summary>
		/// <param name="fs">Open log file stream</param>
		private static void FindEndRootTag(FileStream fs)
		{
			long Position = fs.Length - 3;

			// Find "/" part of end root tag
			while (Position >= 0)
			{
				fs.Seek(Position, SeekOrigin.Begin);
				int b = fs.ReadByte();
				if (b == '/') break;
				Position--;
			}

			// Find "<" part of end root tag
			while (Position >= 0)
			{
				fs.Seek(Position, SeekOrigin.Begin);
				int b = fs.ReadByte();
				if (b == '<') break;
				Position--;
			}

			// Find ">" of previous tag
			while (Position >= 0)
			{
				fs.Seek(Position, SeekOrigin.Begin);
				int b = fs.ReadByte();
				if (b == '>') break;
				Position--;
			}

			if (Position > 0)
				fs.Seek(Position, SeekOrigin.Begin);
		}

		/// <summary>
		/// Expand current log file name: Parameter {0} can be specified and is recalculated on each log add.
		/// </summary>
		protected string ExpandedLogFileName
		{
			get
			{
				// Reformat {0} using current DateTime
				return String.Format(_fileName, DateTime.Now);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get/Set currently active logfile
		/// </summary>
		public string LogFileName
		{
			get { return _fileName; }
			set
			{
				if (string.IsNullOrEmpty(value)) throw new ApplicationException("Invalid filename.");

				int splitPoint = value.LastIndexOf('/');

				if (splitPoint > 0 && HttpContext.Current != null) //web and something to map
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
						fileParts[0] = HttpContext.Current.Server.MapPath(fileParts[0]);

						// join parts.
						_fileName = String.Join(@"\", fileParts);
					}
					catch (HttpException)
					{
						// HttpException is thrown if MapPath is attemtped on a physical filename. 
						// Just ignore it, and save value unchanged.
						_fileName = value;
					}
				}
				else
					_fileName = value;
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

		#endregion

		#region Add 

		/// <summary>
		/// Persist soap log entry to logfile.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="soapMessageType"></param>
		/// <param name="soapStream"></param>
		public void Add(string action, string soapMessageType, Stream soapStream)
		{
			// Load stream into document
			var doc = new XmlDocument();
			soapStream.Position = 0;
			doc.Load(soapStream);
			soapStream.Position = 0;

			// Open log
			FileStream fs = OpenFileStream();
			bool isEmpty = (fs.Length == 0);

			if (!isEmpty) FindEndRootTag(fs);

			var sw = new StreamWriter(fs, _encoding);
			var xw = new XmlTextWriter(sw);
			xw.Formatting = Formatting.Indented;
			xw.IndentChar = '\t';
			xw.Indentation = 1;

			if (isEmpty)
			{
				xw.WriteStartDocument();
				xw.WriteStartElement(_rootElement);
			}
			else
			{
				// Fool writer to think it is inside root node
				long Position = fs.Position;
				xw.WriteStartElement(_rootElement);
				xw.Flush();
				fs.Seek(Position, SeekOrigin.Begin);
			}

			// Write tag around entry
			xw.WriteStartElement(_nodeElement);
			xw.WriteStartAttribute("Action");
			xw.WriteValue(action);
			xw.WriteEndAttribute();
			xw.WriteStartAttribute("Type");
			xw.WriteValue(soapMessageType);
			xw.WriteEndAttribute();
			xw.WriteStartAttribute("TimeStamp");
			xw.WriteValue(DateTime.Now);
			xw.WriteEndAttribute();

			// Write document to stream (raw)
			xw.WriteRaw("\r\n");
			xw.WriteRaw(doc.DocumentElement.OuterXml);
			xw.WriteRaw("\r\n");

			// Close XML Writer (writes end tags)
			xw.Close();
		}

		#endregion
	}
}

#endif