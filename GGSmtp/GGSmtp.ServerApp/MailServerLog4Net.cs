using log4net;
using Simple.MailServer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp.ServerApp
{
    public class Log4NetMailServerLogger : IMailServerLogger
    {
        MailServerLogLevel mLogLevel;
        ILog mLogger;

        public Log4NetMailServerLogger(MailServerLogLevel logLevel)
        {
            mLogLevel = logLevel;
            mLogger = LogManager.GetLogger("MailServerLogger");
        }

        public MailServerLogLevel LogLevel
        {
            get
            {
                return mLogLevel;
            }
        }

        public void Debug(string message)
        {
            if (ShouldLog(MailServerLogLevel.Debug))
            {
                mLogger.Debug(message);
            }
        }

        public void Error(Exception ex)
        {
            if(ShouldLog(MailServerLogLevel.Error))
            {
                mLogger.Error(ex);
            }
        }

        public void Error(string message)
        {
            if (ShouldLog(MailServerLogLevel.Error))
            {
                mLogger.Error(message);
            }
        }

        public void Error(Exception ex, string message)
        {
            if (ShouldLog(MailServerLogLevel.Error))
            {
                mLogger.Error(message, ex);
            }
        }

        public void Info(string message)
        {
            if (ShouldLog(MailServerLogLevel.Info))
            {
                mLogger.Info(message);
            }
        }

        public void Warn(string message)
        {
            if (ShouldLog(MailServerLogLevel.Warn))
            {
                mLogger.Warn(message);
            }
        }

        private bool ShouldLog(MailServerLogLevel level)
        {
            return mLogLevel >= level;
        }
    }
}
