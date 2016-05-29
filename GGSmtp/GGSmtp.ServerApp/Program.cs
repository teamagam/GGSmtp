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
            GGSmtpServer server = new GGSmtpServer(28960, MailServerLogLevel.Debug);

            server.StartServer(false);

            Console.ReadLine();

            server.StopServer();
        }
    }
}
