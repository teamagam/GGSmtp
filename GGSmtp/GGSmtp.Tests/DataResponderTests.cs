using System;
using NUnit.Framework;
using Simple.MailServer.Smtp.Config;
using Moq;
using Simple.MailServer.Smtp;
using System.IO;
using Simple.MailServer;
using System.Text;

namespace GGSmtp.Tests
{
    [TestFixture]
    public class DataResponderTests
    {
        [Test(Author = "Dor", Description = "Checks the DefaultDataResponder", TestOf = typeof(DefaultDataResponder))]
        public void TestAttachmentSend()
        {
            string[] lines = File.ReadAllLines(@"C:\Temp\Mail\2016-05-15_113132_746.eml");

            Mock<ISmtpServerConfiguration> configMock = new Mock<ISmtpServerConfiguration>();
            Mock<ISmtpSessionInfo> sessionInfoMock = new Mock<ISmtpSessionInfo>();

            sessionInfoMock.Setup(mock => mock.MailFrom).Returns(MailAddressWithParameters.Parse("teamagam@gmail.com"));

            DefaultDataResponder responder = new DefaultDataResponder(configMock.Object);

            
            responder.DataStart(sessionInfoMock.Object);

            foreach (string line in lines)
            {
                responder.DataLine(sessionInfoMock.Object, Encoding.ASCII.GetBytes(line));
            }

            SmtpResponse response = responder.DataEnd(sessionInfoMock.Object);

            Assert.IsTrue(response.ResponseCode == 250, "Response result is different from OK, something went wrong");
        }
    }
}
