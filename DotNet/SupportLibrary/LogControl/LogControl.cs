namespace SupportLibrary
{
    #region Using directives.
    // ----------------------------------------------------------------------

    using System;
    using System.Configuration;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;

    using log4net;
    //using log4net.spi;
    using log4net.Appender;
    using log4net.Repository.Hierarchy;

    // ----------------------------------------------------------------------
    #endregion

    /////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Arguments to the log central event.
    /// </summary>
    /// <remarks>	
    /// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
    /// for questions regarding this class.
    /// </remarks>
    public class LogCentralLogEventArgs :
        EventArgs
    {
        #region Constructors.
        // ------------------------------------------------------------------

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralLogEventArgs(
            LogType type,
            object message,
            Exception t)
        {
            this.type = type;
            this.message = message;
            this.error = t;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralLogEventArgs(
            LogType type,
            object message)
        {
            this.type = type;
            this.message = message;
            this.error = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralLogEventArgs(
            LogType type)
        {
            this.type = type;
            this.message = null;
            this.error = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralLogEventArgs()
        {
            this.type = LogType.Unknown;
            this.message = null;
            this.error = null;
        }

        // ------------------------------------------------------------------
        #endregion

        #region Public properties.
        // ------------------------------------------------------------------

        /// <summary>
        /// The type of the texts to log.
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// Unknown type.
            /// </summary>
            Unknown,

            /// <summary>
            /// Info.
            /// </summary>
            Info,

            /// <summary>
            /// Error.
            /// </summary>
            Error,

            /// <summary>
            /// Debug.
            /// </summary>
            Debug,

            /// <summary>
            /// Warning.
            /// </summary>
            Warn,

            /// <summary>
            /// Fatal.
            /// </summary>
            Fatal
        }

        /// <summary>
        /// The type of log event.
        /// </summary>
        public LogType Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// The message that is logged. You cannot modify the
        /// message in your event handler, because the logging
        /// already has occured.
        /// </summary>
        public object Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// The exception that occured.
        /// </summary>
        public Exception Error
        {
            get
            {
                return error;
            }
        }

        // ------------------------------------------------------------------
        #endregion

        #region Private members.
        // ------------------------------------------------------------------

        /// <summary>
        /// Make them private for read-only.
        /// </summary>
        private readonly LogType type;
        private readonly object message;
        private readonly Exception error;

        // ------------------------------------------------------------------
        #endregion
    }

    /// <summary>
    /// Delegate that is called upon logging from LogXxx().
    /// </summary>
    /// <remarks>	
    /// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
    /// for questions regarding this delegate.
    /// </remarks>
    public delegate void LogCentralLogEventHandler(
    object sender,
    LogCentralLogEventArgs e);

    /////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Arguments to the event that tries to query additional information
    /// about the environment or the current session info.
    /// </summary>
    /// <remarks>	
    /// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
    /// for questions regarding this class.
    /// </remarks>
    public class LogCentralRequestMoreInformationEventArgs :
        LogCentralLogEventArgs
    {
        #region Constructors.
        // ------------------------------------------------------------------

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralRequestMoreInformationEventArgs(
            LogType type,
            object message,
            Exception t) :
            base(type, message, t)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralRequestMoreInformationEventArgs(
            LogType type,
            object message) :
            base(type, message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralRequestMoreInformationEventArgs(
            LogType type) :
            base(type)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCentralRequestMoreInformationEventArgs() :
            base()
        {
        }

        // ------------------------------------------------------------------
        #endregion

        #region Public properties.
        // ------------------------------------------------------------------

        /// <summary>
        /// The event handler that gets this object passed can
        /// place additional information here.
        /// </summary>
        public string MoreInformationMessage
        {
            get
            {
                return moreInformationMessage;
            }
            set
            {
                moreInformationMessage = value;
            }
        }

        // ------------------------------------------------------------------
        #endregion

        #region Private member.
        // ------------------------------------------------------------------

        /// <summary>
        /// The event handler that gets this object passed can
        /// place additional information here.
        /// </summary>
        private string moreInformationMessage = null;

        // ------------------------------------------------------------------
        #endregion
    }

    /// <summary>
    /// Delegate that is called when the logging framework tries to query 
    /// additional information about the environment or the current session 
    /// info. Useful e.g. for passing infos about the currently logged in
    /// user or other things.
    /// </summary>
    /// <remarks>	
    /// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
    /// for questions regarding this delegate.
    /// </remarks>
    public delegate void LogCentralRequestMoreInformationEventHandler(
    object sender,
    LogCentralRequestMoreInformationEventArgs e);

    /////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Central base for logging, so that even library functions
    /// have access to logging functionality.
    /// </summary>
    /// <remarks>	
    /// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
    /// for questions regarding this class.
    /// </remarks>
    public class LogCentral
    {
        #region Static member.
        // ------------------------------------------------------------------

        /// <summary>
        /// Access the current instance of the class.
        /// </summary>
        public static LogCentral Current
        {
            get
            {
                lock (typeof(LogCentral))
                {
                    if (current == null)
                    {
                        current = new LogCentral();
                        current.ConfigureLogging();
                    }

                    return current;
                }
            }
        }

        /// <summary>
        /// Can be static here, even for a web application, because
        /// no session-/user-dependent information is stored inside
        /// this class.
        /// </summary>
        private static LogCentral current = null;

        // ------------------------------------------------------------------
        #endregion

        #region Enhanced logging.
        // ------------------------------------------------------------------

        /// <summary>
        /// The event that tries to request more information. This event
        /// is raised before a message is actually being logged. Use this
        /// event to add your own handler to provide more detailed information
        /// to the message being logged.
        /// </summary>
        public event LogCentralRequestMoreInformationEventHandler
            RequestMoreInformation
        {
            add
            {
                if (requestMoreInformationEvents == null)
                {
                    requestMoreInformationEvents = new ArrayList();
                }

                requestMoreInformationEvents.Add(value);
            }
            remove
            {
                if (requestMoreInformationEvents != null)
                {
                    if (requestMoreInformationEvents.IndexOf(value) >= 0)
                    {
                        requestMoreInformationEvents.Remove(value);
                    }
                }
            }
        }

        /// <summary>
        /// Collect the events.
        /// </summary>
        private ArrayList requestMoreInformationEvents;

        /// <summary>
        /// Logging event. This event is raised after
        /// a call to one of the LogDebug, LogError, etc. functions occured.
        /// </summary>
        public event LogCentralLogEventHandler
            Log = null;

        /// <summary>
        /// Log a debug message.
        /// </summary>
        public void LogDebug(
            object message)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Debug,
                        message);
                }

                logImplementation.Debug(message);

                if (WebTrace != null)
                {
                    WebTrace.Write("debug", message == null ? string.Empty : message.ToString());
                }

                Trace.WriteLine(MakeTraceMessage(message), "debug");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Debug,
                        MakeTraceMessage(message)));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        public void LogDebug(
            object message,
            Exception t)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Debug,
                        message,
                        t);
                }

                logImplementation.Debug(message, t);

                if (WebTrace != null)
                {
                    WebTrace.Write("debug", message == null ? string.Empty : message.ToString(), t);
                }

                Trace.WriteLine(MakeTraceMessage(message, t), "debug");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Debug,
                        MakeTraceMessage(message, t),
                        t));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log a fatal message.
        /// </summary>
        public void LogFatal(
            object message)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Fatal,
                        message);
                }

                logImplementation.Fatal(message);

                if (WebTrace != null)
                {
                    WebTrace.Write("fatal", message == null ? string.Empty : message.ToString());
                }

                Trace.WriteLine(MakeTraceMessage(message), "fatal");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Fatal,
                        MakeTraceMessage(message)));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log a fatal message.
        /// </summary>
        public void LogFatal(
            object message,
            Exception t)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Fatal,
                        message,
                        t);
                }

                logImplementation.Fatal(message, t);

                if (WebTrace != null)
                {
                    WebTrace.Write("fatal", message == null ? string.Empty : message.ToString(), t);
                }

                Trace.WriteLine(MakeTraceMessage(message, t), "fatal");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Fatal,
                        MakeTraceMessage(message, t),
                        t));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log an error message.
        /// </summary>
        public void LogError(
            object message)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Error,
                        message);
                }

                logImplementation.Error(message);

                if (WebTrace != null)
                {
                    WebTrace.Write("error", message == null ? string.Empty : message.ToString());
                }

                Trace.WriteLine(MakeTraceMessage(message), "error");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Error,
                        MakeTraceMessage(message)));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log an error message.
        /// </summary>
        public void LogError(
            object message,
            Exception t)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Error,
                        message,
                        t);
                }

                logImplementation.Error(message, t);

                if (WebTrace != null)
                {
                    WebTrace.Write("error", message == null ? string.Empty : message.ToString(), t);
                }

                Trace.WriteLine(MakeTraceMessage(message, t), "error");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Error,
                        MakeTraceMessage(message, t),
                        t));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        public void LogInfo(
            object message)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Info,
                        message);
                }

                logImplementation.Info(message);

                if (WebTrace != null)
                {
                    WebTrace.Write("info", message == null ? string.Empty : message.ToString());
                }

                Trace.WriteLine(MakeTraceMessage(message), "info");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Info,
                        MakeTraceMessage(message)));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        public void LogInfo(
            object message,
            Exception t)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Info,
                        message,
                        t);
                }

                logImplementation.Info(message, t);

                if (WebTrace != null)
                {
                    WebTrace.Write("info", message == null ? string.Empty : message.ToString(), t);
                }

                Trace.WriteLine(MakeTraceMessage(message, t), "info");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Info,
                        MakeTraceMessage(message, t),
                        t));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log a warning message.
        /// </summary>
        public void LogWarn(
            object message)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Warn,
                        message);
                }

                logImplementation.Warn(message);

                if (WebTrace != null)
                {
                    WebTrace.Write("warn", message == null ? string.Empty : message.ToString());
                }

                Trace.WriteLine(MakeTraceMessage(message), "warn");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Warn,
                        MakeTraceMessage(message)));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        /// <summary>
        /// Log a warning message.
        /// </summary>
        public void LogWarn(
            object message,
            Exception t)
        {
            // Never allow passwords to be displayed, no matter where.
            message = ProtectPasswords(message);

            bool isInsideLogging = this.isInsideLogging;
            this.isInsideLogging = true;
            try
            {
                if (!isInsideLogging)
                {
                    message = DispatchRequestMoreInformationEvents(
                        LogCentralLogEventArgs.LogType.Warn,
                        message,
                        t);
                }

                logImplementation.Warn(message, t);

                if (WebTrace != null)
                {
                    WebTrace.Write("warn", message == null ? string.Empty : message.ToString(), t);
                }

                Trace.WriteLine(MakeTraceMessage(message, t), "warn");

                if (!isInsideLogging && Log != null)
                {
                    Log(
                        this,
                        new LogCentralLogEventArgs(
                        LogCentralLogEventArgs.LogType.Warn,
                        MakeTraceMessage(message, t),
                        t));
                }
            }
            finally
            {
                this.isInsideLogging = false;
            }
        }

        // ------------------------------------------------------------------
        #endregion

        #region Logging.
        // ------------------------------------------------------------------

        /// <summary>
        /// Constructor.
        /// </summary>
        private LogCentral()
        {
            // Add default handlers.
            this.RequestMoreInformation +=
                new LogCentralRequestMoreInformationEventHandler(
                LogCentral_RequestMoreInformationHttp);
            this.RequestMoreInformation +=
                new LogCentralRequestMoreInformationEventHandler(
                LogCentral_RequestMoreInformationNetworkEnvironment);
            this.RequestMoreInformation +=
                new LogCentralRequestMoreInformationEventHandler(
                LogCentral_RequestMoreInformationEnvironment);
            this.RequestMoreInformation +=
                new LogCentralRequestMoreInformationEventHandler(
                LogCentral_RequestMoreInformationAssembly);
        }

        /// <summary>
        /// The actual connection to LOG4NET.
        /// </summary>
        private ILog logImplementation = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// By using this flag, we avoid stack overflow inside the called
        /// event handlers during logging.
        /// </summary>
        private bool isInsideLogging = false;

        /// <summary>
        /// Automatically configures logging.
        /// This function is called automatically, too.
        /// </summary>
        public void ConfigureLogging()
        {
            lock (this)
            {
                bool configured = false;

                // --

                // Tells the logging system the correct path.
                Assembly a = Assembly.GetEntryAssembly();

                if (a != null && a.Location != null)
                {
                    string path = a.Location + ".config";

                    if (File.Exists(path))
                    {
                        log4net.Config.DOMConfigurator.Configure(
                            new FileInfo(path));
                        configurationFilePath = path;
                        configured = true;
                    }
                    else
                    {
                        path = FindConfigInPath(Path.GetDirectoryName(a.Location));
                        if (File.Exists(path))
                        {
                            log4net.Config.DOMConfigurator.Configure(
                                new FileInfo(path));
                            configurationFilePath = path;
                            configured = true;
                        }
                    }
                }

                // --

                // Also, try web.config.
                if (!configured)
                {
                   
                    if (System.Web.HttpContext.Current != null &&
                        HttpContext.Current.Server != null &&
                        HttpContext.Current.Request != null)
                    {
                        string path = HttpContext.Current.Server.MapPath(
                            HttpContext.Current.Request.ApplicationPath);

                        path = path.TrimEnd('\\') + "\\Web.config";

                        if (File.Exists(path))
                        {
                            log4net.Config.DOMConfigurator.Configure(
                                new FileInfo(path));
                            configurationFilePath = path;
                            configured = true;
                        }
                    }
                }

                // --

                if (!configured)
                {
                    // Tells the logging system the correct path.
                    a = Assembly.GetExecutingAssembly();

                    if (a != null && a.Location != null)
                    {
                        string path = a.Location + ".config";

                        if (File.Exists(path))
                        {
                            log4net.Config.DOMConfigurator.Configure(
                                new FileInfo(path));
                            configurationFilePath = path;
                            configured = true;
                        }
                        else
                        {
                            path = FindConfigInPath(Path.GetDirectoryName(a.Location));
                            if (File.Exists(path))
                            {
                                log4net.Config.DOMConfigurator.Configure(
                                    new FileInfo(path));
                                configurationFilePath = path;
                                configured = true;
                            }
                        }
                    }
                }

                // --

                if (!configured)
                {
                    // Tells the logging system the correct path.
                    a = Assembly.GetCallingAssembly();

                    if (a != null && a.Location != null)
                    {
                        string path = a.Location + ".config";

                        if (File.Exists(path))
                        {
                            log4net.Config.DOMConfigurator.Configure(
                                new FileInfo(path));
                            configurationFilePath = path;
                            configured = true;
                        }
                        else
                        {
                            path = FindConfigInPath(Path.GetDirectoryName(a.Location));
                            if (File.Exists(path))
                            {
                                log4net.Config.DOMConfigurator.Configure(
                                    new FileInfo(path));
                                configurationFilePath = path;
                                configured = true;
                            }
                        }
                    }
                }

                // --

                if (!configured)
                {
                    // Look in the path where this library is stored.
                    a = Assembly.GetAssembly(typeof(LogCentral));

                    if (a != null && a.Location != null)
                    {
                        string path = FindConfigInPath(Path.GetDirectoryName(a.Location));
                        if (File.Exists(path))
                        {
                            log4net.Config.DOMConfigurator.Configure(
                                new FileInfo(path));
                            configurationFilePath = path;
                            configured = true;
                        }
                    }
                }

                // --
                // Make some changes to the appenders.

                if (configured)
                {
                    // Find all file appenders and make all 
                    // relative paths as absolute paths.

                    Logger logger = logImplementation.Logger as Logger;

                    if (logger != null)
                    {
                        if (logger.Hierarchy.Root.Appenders != null)
                        {
                            foreach (IAppender appender in
                                logger.Hierarchy.Root.Appenders)
                            {
                                FileAppender fileAppender =
                                    appender as FileAppender;

                                if (fileAppender != null)
                                {
                                    // If a relative path is specified,
                                    // make absolute.
                                    if (!Path.IsPathRooted(fileAppender.File))
                                    {
                                        /*
                                        string path = 
                                            Path.GetFullPath(
                                            Path.Combine( 
                                            Path.GetDirectoryName( configurationFilePath ),
                                            fileAppender.File ) );

                                        fileAppender.File = path;
                                        */
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the full path to the configuration file of this
        /// application. Usually a file with ".CONFIG" extension.
        /// </summary>
        public string ConfigurationFilePath
        {
            get
            {
                lock (this)
                {
                    return configurationFilePath;
                }
            }
        }

        /// <summary>
        /// Searches for a configuration file in the given path.
        /// </summary>
        private string FindConfigInPath(
            string path)
        {
            string[] files = Directory.GetFiles(path);

            if (files != null && files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Trim('.').ToLower(
                        CultureInfo.CurrentCulture) == "config")
                    {
                        return file;
                    }
                }
            }

            // Not found.
            return string.Empty;
        }

        /// <summary>
        /// If called from within a web context, this returns non-null.
        /// </summary>
        private System.Web.TraceContext WebTrace
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                {
                    return null;
                }
                else
                {
                    return System.Web.HttpContext.Current.Trace;
                }
            }
        }

        /// <summary>
        /// The path where the configuration is read from.
        /// This value is set upon a call to ConfigureLogging().
        /// </summary>
        private string configurationFilePath;

        /// <summary>
        /// Combines the more information strings and the regular
        /// message string.
        /// </summary>
        private object CombineMoreInformationMessage(
            object message,
            string moreInformationMessage)
        {
            if (moreInformationMessage == null)
            {
                return message;
            }
            else
            {
                if (message == null)
                {
                    return Indent(moreInformationMessage);
                }
                else
                {
                    return message.ToString().TrimEnd() +
                        "\r\n\r\n" +
                        Indent(moreInformationMessage.Trim());
                }
            }
        }

        /// <summary>
        /// Dispatches the RequestMoreInformation event to
        /// all connected event handlers (if any).
        /// </summary>
        private object DispatchRequestMoreInformationEvents(
            LogCentralLogEventArgs.LogType type,
            object message)
        {
            return DispatchRequestMoreInformationEvents(
                type,
                message,
                null);
        }

        /// <summary>
        /// Dispatches the RequestMoreInformation event to
        /// all connected event handlers (if any).
        /// </summary>
        private object DispatchRequestMoreInformationEvents(
            LogCentralLogEventArgs.LogType type,
            object message,
            Exception t)
        {
            if (requestMoreInformationEvents == null ||
                requestMoreInformationEvents.Count <= 0)
            {
                return message;
            }
            else
            {
                object cumulatedMessage = message;

                foreach (LogCentralRequestMoreInformationEventHandler evt
                              in requestMoreInformationEvents)
                {
                    if (evt != null)
                    {
                        LogCentralRequestMoreInformationEventArgs args =
                            new LogCentralRequestMoreInformationEventArgs(
                            type,
                            message,
                            t);

                        evt(this, args);

                        // Yes, add additional information.
                        cumulatedMessage = CombineMoreInformationMessage(
                            cumulatedMessage,
                            args.MoreInformationMessage);
                    }
                }

                return cumulatedMessage;
            }
        }

        // ------------------------------------------------------------------
        #endregion

        #region Pair class.
        // ------------------------------------------------------------------

        /// <summary>
        /// A simple pair, used for name-value pairs.
        /// </summary>
        private class ObjectPair
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public ObjectPair()
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            public ObjectPair(
                object name)
            {
                this.name = name;
                this.value = name;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            public ObjectPair(
                object name,
                object val)
            {
                this.name = name;
                this.value = val;
            }

            /// <summary>
            /// Access the name.
            /// </summary>
            public object Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                }
            }

            /// <summary>
            /// Access the value.
            /// </summary>
            public object Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = value;
                }
            }

            private object name;
            private object value;
        }

        // ------------------------------------------------------------------
        #endregion

        #region Helper.
        // ------------------------------------------------------------------

        /// <summary>
        /// Make a string to trace from a given message and exception.
        /// </summary>
        public static string MakeTraceMessage(
            Exception t)
        {
            return MakeTraceMessage(null, t);
        }

        /// <summary>
        /// Make a string to trace from a given message and exception.
        /// </summary>
        public static string MakeTraceMessage(
            object message,
            Exception t)
        {
            if (message == null)
            {
                if (t == null)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}\r\n\r\n-----------------\r\n\r\n{1}\r\n\r\n-----------------\r\n\r\n{2}",
                        t.GetType() == null || t.GetType().FullName == null ? string.Empty : t.GetType().FullName,
                        t.Message == null ? string.Empty : t.Message.Trim(),
                        t.StackTrace == null ? string.Empty : t.StackTrace.Trim());
                }
            }
            else
            {
                if (t == null)
                {
                    return message.ToString();
                }
                else
                {
                    // Comment.
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}\r\n\r\n{1}\r\n\r\n-----------------\r\n\r\n{2}\r\n\r\n-----------------\r\n\r\n{3}",
                        message,
                        t.GetType() == null || t.GetType().FullName == null ? string.Empty : t.GetType().FullName,
                        t.Message == null ? string.Empty : t.Message.Trim(),
                        t.StackTrace == null ? string.Empty : t.StackTrace.Trim());
                }
            }
        }

        /// <summary>
        /// Make a string to trace from a given message.
        /// </summary>
        public static string MakeTraceMessage(
            object message)
        {
            return message == null ? string.Empty : message.ToString();
        }

        /// <summary>
        /// Never allow passwords to be displayed, no matter where.
        /// </summary>
        public static object ProtectPasswords(
            object message)
        {
            if (message == null || message.ToString().Length <= 0)
            {
                return message;
            }
            else
            {
                string pattern = @"\b(?:password|passwort|kennwort|pwd)\b";

                if (Regex.IsMatch(
                    message.ToString(),
                    pattern,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline))
                {
                    return string.Format(
                        "##### Due to security restrictions (illegal words contained), the message to log has been discarded. The message contained {0} characters.",
                        message.ToString().Length);
                }
                else
                {
                    return message;
                }
            }
        }

        /// <summary>
        /// Indents block of lines.
        /// </summary>
        /// <param name="textToIndent">The textToIndent to indent.</param>
        /// <returns>Returns the indented textToIndent.</returns>
        public static string Indent(
            string textToIndent)
        {
            return Indent(textToIndent, "    ");
        }

        /// <summary>
        /// Indents block of lines.
        /// </summary>
        /// <param name="textToIndent">The textToIndent to indent.</param>
        /// <param name="linePrefix">The prefix to add before every found line.</param>
        /// <returns>Returns the indented textToIndent.</returns>
        public static string Indent(
            string textToIndent,
            string linePrefix)
        {
            if (textToIndent == null)
            {
                return textToIndent;
            }
            else
            {
                textToIndent = textToIndent.Replace("\r\n", "\n");
                textToIndent = textToIndent.Replace("\r", "\n");

                if (textToIndent.IndexOf('\n') < 0)
                {
                    return linePrefix + textToIndent;
                }
                else
                {
                    string[] lines = textToIndent.Split('\n');

                    string result = string.Empty;

                    foreach (string line in lines)
                    {
                        if (result.Length > 0)
                        {
                            result += "\r\n";
                        }

                        result += linePrefix + line;
                    }

                    return result;
                }
            }
        }

        // ------------------------------------------------------------------
        #endregion

        #region Information provider.
        // ------------------------------------------------------------------

        /// <summary>
        /// Provide my own handler for basic information.
        /// </summary>
        /// <remarks>
        /// This handler is called when the logging framework wants more
        /// information about the current environment.
        /// </remarks>
        private void LogCentral_RequestMoreInformationHttp(
            object sender,
            LogCentralRequestMoreInformationEventArgs e)
        {
            if (e.Type == LogCentralLogEventArgs.LogType.Error ||
                e.Type == LogCentralLogEventArgs.LogType.Fatal ||
                e.Type == LogCentralLogEventArgs.LogType.Warn)
            {
                // Provide some HTTP context information.
                if (HttpContext.Current != null)
                {
                    e.MoreInformationMessage = string.Empty;

                    // --
                    // Request object.

                    HttpRequest req = HttpContext.Current.Request;

                    if (req != null)
                    {
                        string infos = string.Format(
                            CultureInfo.CurrentCulture,
                            "Absolute URI: {0},\r\n" +
                            "Referer URI: {1},\r\n" +
                            "User host address: {2},\r\n" +
                            "User host name: {3},\r\n" +
                            "User agent: {4},\r\n" +
                            "HTTP method: {5}."
                            ,
                            req.Url == null || req.Url.AbsoluteUri == null ? "(null)" : req.Url.AbsoluteUri,
                            req.UrlReferrer == null || req.UrlReferrer.AbsoluteUri == null ? "(null)" : req.UrlReferrer.AbsoluteUri,
                            req.UserHostAddress == null ? "(null)" : req.UserHostAddress,
                            req.UserHostName == null ? "(null)" : req.UserHostName,
                            req.UserAgent == null ? "(null)" : req.UserAgent,
                            req.HttpMethod == null ? "(null)" : req.HttpMethod);

                        if (e.MoreInformationMessage.Length > 0)
                        {
                            e.MoreInformationMessage += "\r\n";
                        }
                        e.MoreInformationMessage += infos;
                    }

                    // --
                    // Page object.

                    System.Web.UI.Page page =
                        HttpContext.Current.Handler as System.Web.UI.Page;

                    if (page != null && req != null)
                    {
                        string viewStateKey = page.ViewStateUserKey;
                        if (viewStateKey == null || viewStateKey.Length <= 0)
                        {
                            viewStateKey = "__VIEWSTATE";
                        }

                        string viewStateValue = null;
                        int viewStateValueLength = 0;
                        if (viewStateKey != null)
                        {
                            viewStateValue = req[viewStateKey];

                            if (viewStateValue != null)
                            {
                                viewStateValueLength = viewStateValue.Length;
                            }

                            if (viewStateValue != null && viewStateValue.Length > 200)
                            {
                                viewStateValue = viewStateValue.Substring(0, 200) + "... (cut)";
                            }
                        }

                        string infos = string.Format(
                            CultureInfo.CurrentCulture,
                            "View state key: {0}\r\n" +
                            "View state value length: {1} characters\r\n" +
                            "View state value: {2}."
                            ,
                            viewStateKey == null ? "(null)" : viewStateKey,
                            viewStateValueLength,
                            viewStateValue == null ? "(null)" : viewStateValue
                            );

                        if (e.MoreInformationMessage.Length > 0)
                        {
                            e.MoreInformationMessage += "\r\n\r\n";
                        }
                        e.MoreInformationMessage += infos;
                    }

                    // --
                    // Session object.

                    System.Web.SessionState.HttpSessionState session =
                        HttpContext.Current.Session;

                    if (session != null)
                    {
                        string infos = string.Format(
                            CultureInfo.CurrentCulture,
                            "Session ID: {0}\r\n" +
                            "Is new session: {1}\r\n" +
                            "Is cookieless session: {2}\r\n" +
                            "Session element count: {3}\r\n" +
                            "Session mode: {4}\r\n" +
                            "Session timeout: {5}."
                            ,
                            session.SessionID == null ? "(null)" : session.SessionID,
                            session.IsNewSession,
                            session.IsCookieless,
                            session.Count,
                            session.Mode,
                            session.Timeout
                            );

                        if (e.MoreInformationMessage.Length > 0)
                        {
                            e.MoreInformationMessage += "\r\n\r\n";
                        }
                        e.MoreInformationMessage += infos;
                    }
                }
            }
        }

        /// <summary>
        /// Provide my own handler for basic information.
        /// </summary>
        /// <remarks>
        /// This handler is called when the logging framework wants more
        /// information about the current environment.
        /// </remarks>
        private void LogCentral_RequestMoreInformationAssembly(
            object sender,
            LogCentralRequestMoreInformationEventArgs e)
        {
            if (e.Type == LogCentralLogEventArgs.LogType.Error ||
                e.Type == LogCentralLogEventArgs.LogType.Fatal ||
                e.Type == LogCentralLogEventArgs.LogType.Warn)
            {
                ArrayList assemblies = new ArrayList();
                assemblies.Add(new ObjectPair("Entry assembly", Assembly.GetEntryAssembly()));
                assemblies.Add(new ObjectPair("Executing assembly", Assembly.GetExecutingAssembly()));
                assemblies.Add(new ObjectPair("Calling assembly", Assembly.GetCallingAssembly()));

                string infos = string.Empty;

                foreach (ObjectPair pair in assemblies)
                {
                    string assemblyType = pair.Name.ToString();
                    Assembly assembly = pair.Value as Assembly;

                    if (assembly != null)
                    {
                        string info = string.Format(
                            CultureInfo.CurrentCulture,
                            "Assembly type: {0},\r\n" +
                            "Assembly full name: {1},\r\n" +
                            "Assembly location: {2},\r\n" +
                            "Assembly date: {3},\r\n" +
                            "Assembly version: {4}."
                            ,
                            assemblyType,
                            assembly.FullName == null ? "(null)" : assembly.FullName,
                            assembly.Location == null ? "(null)" : assembly.Location,
                            assembly.Location == null ? "(null)" : File.GetLastWriteTime(assembly.Location).ToString(CultureInfo.CurrentCulture),
                            assembly.GetName() == null || assembly.GetName().Version == null ? "(null)" : assembly.GetName().Version.ToString());

                        if (infos.Length > 0)
                        {
                            infos += "\r\n\r\n";
                        }

                        infos += info;
                    }
                }

                if (infos.Length > 0)
                {
                    e.MoreInformationMessage = infos;
                }
            }
        }

        /// <summary>
        /// Provide my own handler for basic information.
        /// </summary>
        /// <remarks>
        /// This handler is called when the logging framework wants more
        /// information about the current environment.
        /// </remarks>
        private void LogCentral_RequestMoreInformationEnvironment(
            object sender,
            LogCentralRequestMoreInformationEventArgs e)
        {
            if (e.Type == LogCentralLogEventArgs.LogType.Error ||
                e.Type == LogCentralLogEventArgs.LogType.Fatal ||
                e.Type == LogCentralLogEventArgs.LogType.Warn)
            {
                // Provide some environment context information.

                string infos = string.Format(
                    CultureInfo.CurrentCulture,
                    "User domain name: {0},\r\n" +
                    "User name: {1},\r\n" +
                    "Machine name: {2},\r\n" +
                    "OS version: {3},\r\n" +
                    "CLR version: {4},\r\n" +
                    "Command line: {5},\r\n" +
                    "Current directory: {6}.",
                    Environment.UserDomainName == null ? "(null)" : Environment.UserDomainName,
                    Environment.UserName == null ? "(null)" : Environment.UserName,
                    Environment.MachineName == null ? "(null)" : Environment.MachineName,
                    Environment.OSVersion == null ? "(null)" : Environment.OSVersion.ToString(),
                    Environment.Version == null ? "(null)" : Environment.Version.ToString(),
                    Environment.CommandLine == null ? "(null)" : Environment.CommandLine,
                    Environment.CurrentDirectory == null ? "(null)" : Environment.CurrentDirectory);

                e.MoreInformationMessage = infos;
            }
        }

        /// <summary>
        /// Provide my own handler for basic information.
        /// </summary>
        /// <remarks>
        /// This handler is called when the logging framework wants more
        /// information about the current environment.
        /// </remarks>
        private void LogCentral_RequestMoreInformationNetworkEnvironment(
            object sender,
            LogCentralRequestMoreInformationEventArgs e)
        {
            if (e.Type == LogCentralLogEventArgs.LogType.Error ||
                e.Type == LogCentralLogEventArgs.LogType.Fatal ||
                e.Type == LogCentralLogEventArgs.LogType.Warn)
            {
                string hostName = Dns.GetHostName();

                string infos = string.Format(
                    CultureInfo.CurrentCulture,
                    "Host name: {0}",
                    hostName == null ? "(null)" : hostName);

                if (hostName != null)
                {
                    IPHostEntry hostEntry = Dns.Resolve(hostName);

                    if (hostEntry != null)
                    {
                        foreach (IPAddress ipAddress in hostEntry.AddressList)
                        {
                            infos += string.Format(
                                CultureInfo.CurrentCulture,
                                ",\r\nIP address: {0}",
                                ipAddress == null ? "(null)" : ipAddress.ToString());
                        }
                    }
                }

                infos = infos.TrimEnd().TrimEnd(',', '.').TrimEnd() + ".";

                e.MoreInformationMessage = infos;
            }
        }

        // ------------------------------------------------------------------
        #endregion
    }

    /////////////////////////////////////////////////////////////////////////
}