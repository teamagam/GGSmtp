using log4net;
using Simple.MailServer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp.ServerApp
{
    public class MailServerLog4Net : IMailServerLogger
    {
        MailServerLogLevel mLogLevel;
        ILog mLogger;

        public MailServerLog4Net(MailServerLogLevel logLevel)
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
            if (mLogLevel >= MailServerLogLevel.Debug)
            {
                mLogger.Debug(message);
            }
        }

        public void Error(Exception ex)
        {
            if(mLogLevel >= MailServerLogLevel.Error)
            {
                mLogger.Error(ex);
            }
        }

        public void Error(string message)
        {
            if (mLogLevel >= MailServerLogLevel.Error)
            {
                mLogger.Error(message);
            }
        }

        public void Error(Exception ex, string message)
        {
            if (mLogLevel >= MailServerLogLevel.Error)
            {
                mLogger.Error(message, ex);
            }
        }

        public void Info(string message)
        {
            if (mLogLevel >= MailServerLogLevel.Info)
            {
                mLogger.Info(message);
            }
        }

        public void Warn(string message)
        {
            if (mLogLevel >= MailServerLogLevel.Warn)
            {
                mLogger.Warn(message);
            }
        }
    }
}
