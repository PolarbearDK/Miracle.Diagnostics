using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Miracle.Diagnostics.Logging;
using NUnit.Framework;

namespace Miracle.Diagnostics.Test
{
    [XmlRoot("Log")]
    public class TestLog
    {
        [XmlElement("LogEntry", Type = typeof(LogEntry))]
        [XmlElement("ExceptionLogEntry", Type = typeof(ExceptionLogEntry))]
        public LogEntry[] LogEntries { get; set; }
    }


    [TestFixture]
    public class XmlLogTests
    {
        [SetUp]
        public void Setup()
        {
            Filename = Path.Combine(Path.GetTempPath(), string.Format("LogUnitTest{0:yyyyMMddHHmmss}.tmp", DateTime.Now));
        }

        public string Filename { get; set; }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(Filename)) 
                File.Delete(Filename);
        }

        public LogEntry[] GetLogContent()
        {
            using (var xmlReader = XmlReader.Create(Filename))
            {
                var xmlSerializer = new XmlSerializer(typeof (TestLog));
                xmlSerializer.UnknownAttribute += UnknownAttribute;
                xmlSerializer.UnknownElement += UnknownElement;
                xmlSerializer.UnknownNode += UnknownNode;
                var log = (TestLog) xmlSerializer.Deserialize(xmlReader);

                return log != null
                    ? log.LogEntries
                    : null;
            }
        }

        private static void UnknownNode(object sender, XmlNodeEventArgs e)
        {
            throw new ApplicationException(string.Format("Unknown node:{0} while deserializing ILogEntry", e.Name));
        }

        private static void UnknownElement(object sender, XmlElementEventArgs e)
        {
            throw new ApplicationException(string.Format("Unknown element:{0} while deserializing ILogEntry", e.Element.Name));
        }

        private static void UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            throw new ApplicationException(string.Format("Unknown attribute:{0} while deserializing ILogEntry {1}", e.Attr.Name));
        }

        [Test]
        public void TestSingleLogContent()
        {
            ILog log = new XmlLogFile(Filename);

            var originalLogEntry = new LogEntry(SeverityEnum.Information, "Hello world");
            log.Add(originalLogEntry);

            var logEntries = GetLogContent();
            Assert.That(logEntries, Is.Not.Null);
            Assert.That(logEntries, Has.Length.EqualTo(1));
            var entry = logEntries.First();
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.Message, Is.EqualTo(originalLogEntry.Message));
            Assert.That(entry.Severity, Is.EqualTo(originalLogEntry.Severity));
            Assert.That(entry.TimeStamp, Is.EqualTo(originalLogEntry.TimeStamp).Within(TimeSpan.FromMilliseconds(1)));
        }

        private Exception GetException(string message)
        {
            try
            {
                throw new ApplicationException(message);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private Exception GetNestedException(string outerMessage, string innerMessage)
        {
            try
            {
                throw new ApplicationException(outerMessage, GetException(innerMessage));
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        [Test]
        public void TestMixedLogContent()
        {
            ILog log = new XmlLogFile(Filename);

            var originalLogEntry = new LogEntry(SeverityEnum.Information, "Hello 1");
            log.Add(originalLogEntry);

            var logEntries = GetLogContent();
            Assert.That(logEntries, Is.Not.Null);
            Assert.That(logEntries, Has.Length.EqualTo(1));
            var logEntry = logEntries.First();
            Assert.That(logEntry, Is.Not.Null);
            Assert.That(logEntry.Message, Is.EqualTo(originalLogEntry.Message));
            Assert.That(logEntry.Severity, Is.EqualTo(originalLogEntry.Severity));
            Assert.That(logEntry.TimeStamp, Is.EqualTo(originalLogEntry.TimeStamp).Within(TimeSpan.FromMilliseconds(1)));

            Exception exception = GetException("My exception message");
            var originalLogEntry2 = new ExceptionLogEntry(SeverityEnum.Warning, exception);
            log.Add(originalLogEntry2);

            logEntries = GetLogContent();
            Assert.That(logEntries, Is.Not.Null);
            Assert.That(logEntries, Has.Length.EqualTo(2));
            var exceptionLogEntry = logEntries.Last() as ExceptionLogEntry;
            Assert.That(exceptionLogEntry, Is.Not.Null);
            Assert.That(exceptionLogEntry.Message, Is.EqualTo(originalLogEntry2.Message));
            Assert.That(exceptionLogEntry.Severity, Is.EqualTo(originalLogEntry2.Severity));
            Assert.That(exceptionLogEntry.TimeStamp, Is.EqualTo(originalLogEntry2.TimeStamp).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(exceptionLogEntry.TimeStamp, Is.GreaterThan(logEntry.TimeStamp));
            Assert.That(exceptionLogEntry.Exception.Message, Is.EqualTo(exception.Message));

            var originalLogEntry3 = new LogEntry(SeverityEnum.Error, "My oh my");
            log.Add(originalLogEntry3);

            logEntries = GetLogContent();
            Assert.That(logEntries, Is.Not.Null);
            Assert.That(logEntries, Has.Length.EqualTo(3));
            logEntry = logEntries.Last();
            Assert.That(logEntry, Is.Not.Null);
            Assert.That(logEntry.Message, Is.EqualTo(originalLogEntry3.Message));
            Assert.That(logEntry.Severity, Is.EqualTo(originalLogEntry3.Severity));
            Assert.That(logEntry.TimeStamp, Is.EqualTo(originalLogEntry3.TimeStamp).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(logEntry.TimeStamp, Is.GreaterThan(exceptionLogEntry.TimeStamp));
        }
        [Test]
        public void TestNestedException()
        {
            const string inner = "My inner message";
            const string outer= "My outer message";

            ILog log = new XmlLogFile(Filename);

            Exception exception = GetNestedException(outer, inner);
            var logEntry = new ExceptionLogEntry(SeverityEnum.Fatal, exception);
            log.Add(logEntry);

            var logEntries = GetLogContent();
            Assert.That(logEntries, Is.Not.Null);
            Assert.That(logEntries, Has.Length.EqualTo(1));
            var exceptionLogEntry = logEntries.First() as ExceptionLogEntry;
            Assert.That(exceptionLogEntry, Is.Not.Null);
            Assert.That(exceptionLogEntry.Message, Is.EqualTo(inner));
            Assert.That(exceptionLogEntry.Severity, Is.EqualTo(logEntry.Severity));
            Assert.That(exceptionLogEntry.TimeStamp, Is.EqualTo(logEntry.TimeStamp).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(exceptionLogEntry.Exception.Message, Is.EqualTo(outer));
            Assert.That(exceptionLogEntry.Exception.InnerException.Message, Is.EqualTo(inner));
        }
    }
}
