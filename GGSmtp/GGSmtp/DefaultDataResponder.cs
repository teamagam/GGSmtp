using GGSmtp.Properties;
using MimeKit;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using Simple.MailServer.Logging;
using Simple.MailServer.Smtp;
using Simple.MailServer.Smtp.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp
{
    public class DefaultDataResponder : SmtpDataResponder
    {
        private IDictionary<ISmtpSessionInfo, Stream> mSessionsData;
        private IMailServerLogger mLogger;
        private Settings mSettings;

        public DefaultDataResponder(ISmtpServerConfiguration config) :
            base(config)
        {
            mSessionsData = new Dictionary<ISmtpSessionInfo, Stream>();
            mLogger = MailServerLogger.Instance;
            mSettings = Settings.Default;
        }

        public override SmtpResponse DataStart(ISmtpSessionInfo sessionInfo)
        {
            mLogger.Info(string.Format("Start receiving mail from: {0}", sessionInfo.MailFrom));

            mSessionsData[sessionInfo] = new MemoryStream();

            return SmtpResponses.DataStart;
        }

        /// <summary>
        /// Writes "a line" of data to a stream from the SMTP message
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="lineBuf"></param>
        /// <returns></returns>
        public override SmtpResponse DataLine(ISmtpSessionInfo sessionInfo, byte[] lineBuf)
        {
            Stream currentStream = mSessionsData[sessionInfo];

            currentStream.Seek(0, SeekOrigin.End);
            currentStream.Write(lineBuf, 0, lineBuf.Length);

            currentStream.WriteByte(13);
            currentStream.WriteByte(10);

            return SmtpResponses.None;
        }

        public override SmtpResponse DataEnd(ISmtpSessionInfo sessionInfo)
        {
            Stream currentStream = mSessionsData[sessionInfo];
            long streamLength = currentStream.Length;
            string successMessage = string.Format("{0} bytes received", streamLength);

            // Write a success response to return, but don't return it yet
            SmtpResponse response = SmtpResponses.OK.CloneAndChange(successMessage);

            try
            {
                currentStream.Seek(0, SeekOrigin.Begin);

                // Read the mail message from the stream
                MimeMessage message = MimeMessage.Load(currentStream);

                foreach (MimePart attatchment in message.Attachments)
                {
                    mLogger.Debug(string.Format("Got attachment from {0}, sending . . .", sessionInfo.MailFrom));

                    bool success = SendAttachment(sessionInfo.MailFrom.MailAddress, attatchment);

                    if(!success)
                    {
                        mLogger.Error("An error has occoured while sending the message to the server");

                        return SmtpResponses.InternalServerError;
                    }
                }
            }
            catch (Exception ex)
            {
                // Write the error to the log, and return InternalServerError to the user
                mLogger.Error(ex.StackTrace);

                return SmtpResponses.InternalServerError;
            }

            currentStream.Dispose();
            mSessionsData.Remove(sessionInfo);

            mLogger.Info(string.Format("Mail received ({0} bytes): {1}", streamLength, sessionInfo.MailFrom));

            return response;
        }


        /// <summary>
        /// Sends an attachment to the GGMessaging server
        /// </summary>
        /// <param name="attatchment">A mail message attachment</param>
        private bool SendAttachment(string mailFrom, MimePart attatchment)
        {
            string attachmentFileName = attatchment.FileName;
            GGMessage message = new GGMessage()
            {
                SenderId = mailFrom,
                Content = new GGImageMetadata()
                {
                    HasLocation = false,
                    TimeStamp = DateTime.Now.Ticks
                }
            };

            using (MemoryStream attachmentStream = new MemoryStream())
            {
                // Decode the attachment stream to binary stream that can be read or open
                attatchment.ContentObject.DecodeTo(attachmentStream);

                // Create json string from the message object
                string messageJson = JsonConvert.SerializeObject(message);

                // Create the request as multipart
                RestRequest request = new RestRequest(mSettings.GGImagesRequestUrl, Method.POST);

                // Add file the to request, then notify the request object that it's multipart only request.
                request.AddFile("image", attachmentStream.ToArray(), attachmentFileName);
                request.AlwaysMultipartFormData = true;

                request.AddParameter("message", messageJson, "application/json", ParameterType.RequestBody);
                request.AddHeader("Content-Type", "multipart/form-data");

                RestClient client = new RestClient(mSettings.GGMessagingUrl);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
            }

            return true;
        }
    }
}
