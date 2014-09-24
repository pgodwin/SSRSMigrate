using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;

namespace SSRSMigrate.TestHelper.Logging
{
    /// <summary>
    /// Dummy ILogger implementation that does nothing.
    /// </summary>
    public class MockLogger : ILogger
    {
        public void Debug(Exception exception, string format, params object[] args)
        {
            
        }

        public void Debug(string format, params object[] args)
        {
        }

        public void Debug(string message)
        {
        }

        public void DebugException(string message, Exception exception)
        {
        }

        public void Error(Exception exception, string format, params object[] args)
        {
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Error(string message)
        {
        }

        public void ErrorException(string message, Exception exception)
        {
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
        }

        public void Fatal(string format, params object[] args)
        {
        }

        public void Fatal(string message)
        {
        }

        public void FatalException(string message, Exception exception)
        {
        }

        public void Info(Exception exception, string format, params object[] args)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Info(string message)
        {
        }

        public void InfoException(string message, Exception exception)
        {
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsTraceEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public string Name
        {
            get { return "MockLogger"; }
        }

        public void Trace(Exception exception, string format, params object[] args)
        {
        }

        public void Trace(string format, params object[] args)
        {
        }

        public void Trace(string message)
        {
        }

        public void TraceException(string message, Exception exception)
        {
        }

        public Type Type
        {
            get { return null; }
        }

        public void Warn(Exception exception, string format, params object[] args)
        {
        }

        public void Warn(string format, params object[] args)
        {
        }

        public void Warn(string message)
        {
        }

        public void WarnException(string message, Exception exception)
        {
        }
    }
}
