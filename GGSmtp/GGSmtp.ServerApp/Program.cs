using GGSmtp.ServerApp.Properties;
using Simple.MailServer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp.ServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            short port = Settings.Default.Port;
            MailServerLogLevel verbosity = Settings.Default.LogVerbosity;

            GGSmtpServer server = new GGSmtpServer(port, verbosity);

            server.StartServer(false);

            Console.ReadLine();

            server.StopServer();
        }
    }
}
