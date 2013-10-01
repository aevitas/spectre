using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Spectre.Common
{

    /// <summary>
    /// Generic logging class to handle all logging needs.
    /// </summary>
    public static class Logging
    {
        private static readonly Thread LoggingThread;

        private static readonly ConcurrentQueue<LogMessage> LogQueue;

        public delegate void LoggingDelegate(LogMessage message);

        public delegate void ExceptionDelegate(Exception ex);

        public static event LoggingDelegate OnLog;

        public static event ExceptionDelegate OnException;

        public static LogLevel LoggingLevel { get; set; }
        public static string LogPath { get; set; }

        public static string LogDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                                         "Logs");

        private static string _lastMessage;

        #region Constructor

        static Logging()
        {
            LogQueue = new ConcurrentQueue<LogMessage>();
            LoggingLevel = LogLevel.Normal;

#if DEBUG
            LoggingLevel = LogLevel.Diagnostic;
            Debug.Listeners.Add(new LoggingTraceListener());
            Trace.Listeners.Add(new LoggingTraceListener());
#endif

            var now = DateTime.Now;
            LogPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                string.Format("Logs\\{0:0000}-{1:00}-{2:00} {3:00}.{4:00}.txt", now.Year, now.Month, now.Day, now.Hour, now.Minute));



            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            // Someone set us up the logging thread!
            LoggingThread = new Thread(ProcessMessageQueue) { Name = "Logging Thread", IsBackground = true };

            LoggingThread.Start();
        }

        #endregion

        #region Processor

        private static void ProcessMessageQueue()
        {
            while (true)
            {
                try
                {
                    if (LogQueue.Count <= 0)
                        continue;

                    using (var sw = new StreamWriter(LogPath, true))
                    {
                        while (LogQueue.Count > 0)
                        {
                            LogMessage m;
                            if (!LogQueue.TryDequeue(out m))
                                continue;

                            sw.WriteLine(m);
                            InvokeOnLog(m);

                            sw.Flush();
                        }
                    }

                    Thread.Sleep(100);
                }
                catch
                {
                    // Cool exception, bro.
                }
            }
        }

        #endregion

        private static void InvokeOnLog(LogMessage message)
        {
            LoggingDelegate handler = OnLog;
            if (handler != null) handler(message);
        }

        private static void InvokeOnException(Exception ex)
        {
            ExceptionDelegate handler = OnException;
            if (handler != null) handler(ex);
        }

        #region Writing Methods

        /// <summary>
        /// Writes the specified messsage at the specified log level to the log.
        /// </summary>
        public static void Write(LogLevel level, string format, params object[] args)
        {
            if (string.IsNullOrEmpty(format))
                return;

            string s = string.Format(format, args);

            if (s == _lastMessage || string.IsNullOrEmpty(s))
                return;

            if (LoggingLevel == LogLevel.None)
                return;

            LogQueue.Enqueue(new LogMessage(level, string.Format(CultureInfo.InvariantCulture, format, args), DateTime.Now));

            _lastMessage = s;
        }

        /// <summary>
        /// Writes the specified message to the log.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string format, params object[] args)
        {
            Write(LogLevel.Normal, format, args);
        }

        /// <summary>
        /// Writes the specified diagnostic message to the log.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteDiagnostic(string format, params object[] args)
        {
            Write(LogLevel.Diagnostic, format, args);
        }

        /// <summary>
        /// Writes the specified verbose message to the log.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteVerbose(string format, params object[] args)
        {
            Write(LogLevel.Verbose, format, args);
        }

        /// <summary>
        /// Writes the specified exception to the log, and if specified - and if any - the inner exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="includeInner"></param>
        public static void WriteException(Exception ex, bool includeInner = true)
        {
            // We don't need this trash!
            if (ex is ThreadAbortException)
                return;

            WriteDiagnostic(ex.ToString());

            if (ex.InnerException != null && includeInner)
                WriteDiagnostic(ex.InnerException.ToString());

            InvokeOnException(ex);
        }

        /// <summary>
        /// Writes the specified message to the log, but doesn't raise any events in the process. This means it won't show up on
        /// the user interface, but will appear in the log file.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteSilent(string format, params object[] args)
        {
            string s = string.Format(format, args);

            if (s == _lastMessage || string.IsNullOrEmpty(s))
                return;

            LogQueue.Enqueue(new LogMessage(LogLevel.Diagnostic, string.Format(format, args), DateTime.Now));
        }

        /// <summary>
        /// Writes the specified exception to the file, silently.
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteSilent(Exception ex)
        {
            // Fuck it off!
            if (ex is ThreadAbortException)
                return;

            WriteSilent(ex.ToString());
        }

        #endregion

        #region Nested Type : LoggingTraceListener

        class LoggingTraceListener : TraceListener
        {
            #region Overrides of TraceListener

            /// <summary>
            /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
            /// </summary>
            /// <param name="message">A message to write. </param><filterpriority>2</filterpriority>
            public override void Write(string message)
            {
                WriteDiagnostic(message);
            }

            /// <summary>
            /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
            /// </summary>
            /// <param name="message">A message to write. </param><filterpriority>2</filterpriority>
            public override void WriteLine(string message)
            {
                Write(message);
            }

            #endregion
        }

        #endregion

        #region Nested Type : LogMessage

        public class LogMessage
        {
            public LogLevel Level { get; set; }
            public string Message { get; set; }
            public DateTime TimeStamp { get; set; }

            public LogMessage(LogLevel level, string message, DateTime timeStamp)
            {
                Level = level;
                Message = message;
                TimeStamp = timeStamp;
            }

            public string LogStamp { get { return TimeStamp.ToString("HH:mm:ss.fff"); } }

            public override string ToString()
            {
                return string.Format("[{0} {2}] {1}", TimeStamp.ToString("HH:mm:ss.fff"), Message, Level.ToString()[0]);
            }

            public string PlainMessage
            {
                get { return Message; }
            }
        }

        #endregion
    }

    /// <summary>
    /// The level of logging; what's shown in the display log of TankLeader.
    /// Even if for example Normal is specified, all Diagnostic logging is written to file regardless.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Normal logging, no debug stuff.
        /// </summary>
        Normal,
        /// <summary>
        /// Diagnostic, includes debugging information and errors.
        /// </summary>
        Diagnostic,
        /// <summary>
        /// Includes everything, even stuff like compiling profiles.
        /// </summary>
        Verbose,
        /// <summary>
        /// No logging whatsoever. File logging still resumes normally.
        /// </summary>
        None
    }
}
