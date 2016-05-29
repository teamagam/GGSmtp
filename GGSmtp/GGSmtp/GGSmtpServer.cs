using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GGSmtp.Properties;
using Simple.MailServer;
using Simple.MailServer.Logging;
using Simple.MailServer.Smtp;

namespace GGSmtp
{
    public class GGSmtpServer : IDisposable
    {
        private short mPort;
        private bool isStopped;
        private bool mDisposedValue;
        private SmtpServer mSmtpServer;
        private PortListener mPortListener;
        private IRespondToSmtpData mReponseHandler;
        private Settings mSettings;

        /// <summary>
        /// Initializes SMTP server
        /// </summary>
        /// <param name="port">The port to listen to</param>
        /// <param name="responseHandler">
        ///     Custom handler that handles the response from the SMTP server. Can be null, and if so, a default implementation will be used.
        /// </param>
        public GGSmtpServer(short port, IRespondToSmtpData responseHandler = null)
        {
            mPort = port;
            mReponseHandler = responseHandler;
            mSettings = Settings.Default;

            isStopped = true;
            mDisposedValue = false;
        }

        /// <summary>
        /// Initializes SMTP server
        /// </summary>
        /// <param name="port">The port to listen to</param>
        /// <param name="logLevel">The log level to beused in the default ConsoleLogger</param>
        /// <param name="responseHandler">
        ///     Custom handler that handles the response from the SMTP server. Can be null, and if so, a default implementation will be used.
        /// </param>
        public GGSmtpServer(short port, MailServerLogLevel logLevel, IRespondToSmtpData responseHandler = null) :
            this(port, responseHandler)
        {
            MailServerLogger.Set(new MailServerConsoleLogger(logLevel));
        }

        /// <summary>
        /// Initializes SMTP server
        /// </summary>
        /// <param name="port">The port to listen to</param>
        /// <param name="logger">An instance of logger to be used, will relplace the deafult logger</param>
        /// <param name="responseHandler">
        ///     Custom handler that handles the response from the SMTP server. Can be null, and if so, a default implementation will be used.
        /// </param>
        public GGSmtpServer(short port, IMailServerLogger logger, IRespondToSmtpData responseHandler = null) :
            this(port, responseHandler)
        {
            MailServerLogger.Set(logger);
        }

        /// <summary>
        /// Gets the port the server will use.
        /// </summary>
        public short Port
        {
            get { return mPort; }
        }

        /// <summary>
        /// Gets or sets the global conneciton timeout.
        /// </summary>
        public TimeSpan? GlobalConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the connection idle timeout.
        /// Will set how much time the server will send timeout to the client when they're idle.
        /// </summary>
        public TimeSpan? ConnectionIdleTimeout { get; set; }

        /// <summary>
        /// A blocking method to start a server.
        /// </summary>
        public void StartServer(bool blocking)
        {
            mSmtpServer = CreateServer();
            mPortListener = mSmtpServer.BindAndListenTo(IPAddress.Any, mPort);

            if (blocking)
            {
                isStopped = false;
                while (isStopped) { }
            }
        }

        /// <summary>
        /// Stops the server and closes the connection
        /// </summary>
        public void StopServer()
        {
            if (!isStopped)
            {
                isStopped = true;
            }

            mPortListener.StopListen();
        }

        /// <summary>
        /// Creates a server intance
        /// </summary>
        /// <returns>The SmtpServer instance </returns>
        private SmtpServer CreateServer()
        {
            SmtpServer smtpServer = new SmtpServer
            {
                Configuration =
                {
                    DefaultGreeting = mSettings.GreetingMessage
                }
            };

            mReponseHandler = mReponseHandler ?? new DefaultDataResponder(smtpServer.Configuration);

            SmtpResponderFactory responderFactory = new SmtpResponderFactory(smtpServer.Configuration)
            {
                DataResponder = mReponseHandler

            };

            smtpServer.DefaultResponderFactory = responderFactory;

            // Edit the timeouts values if there is a value in it.
            // Otherwise, keep it at default value.
            if(GlobalConnectionTimeout.HasValue)
                smtpServer.Configuration.GlobalConnectionTimeout = GlobalConnectionTimeout.Value;

            if(ConnectionIdleTimeout.HasValue)
                smtpServer.Configuration.ConnectionIdleTimeout = ConnectionIdleTimeout.Value;

            return smtpServer;
        }

        #region IDisposable Support (Disposable Pattern)

        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposedValue)
            {
                if (disposing)
                {
                    StopServer();

                    mPortListener.Dispose();
                    mSmtpServer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                mDisposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
