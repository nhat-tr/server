﻿using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Bit.Core.Services
{
    public class SmtpMailDeliveryService : IMailDeliveryService
    {
        private readonly GlobalSettings _globalSettings;
        private readonly ILogger<SmtpMailDeliveryService> _logger;

        public SmtpMailDeliveryService(
            GlobalSettings globalSettings,
            ILogger<SmtpMailDeliveryService> logger)
        {
            if(globalSettings.Mail?.Smtp?.Host == null)
            {
                throw new ArgumentNullException(nameof(globalSettings.Mail.Smtp.Host));
            }

            _globalSettings = globalSettings;
            _logger = logger;
        }

        public Task SendEmailAsync(Models.Mail.MailMessage message)
        {
            var client = new SmtpClient(_globalSettings.Mail.Smtp.Host, _globalSettings.Mail.Smtp.Port);
            client.EnableSsl = _globalSettings.Mail.Smtp.Ssl;
            client.UseDefaultCredentials = _globalSettings.Mail.Smtp.UseDefaultCredentials;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            if(!string.IsNullOrWhiteSpace(_globalSettings.Mail.Smtp.Username) &&
                !string.IsNullOrWhiteSpace(_globalSettings.Mail.Smtp.Password))
            {
                client.Credentials = new NetworkCredential(_globalSettings.Mail.Smtp.Username,
                    _globalSettings.Mail.Smtp.Password);
            }

            var smtpMessage = new MailMessage();
            smtpMessage.From = new MailAddress(_globalSettings.Mail.ReplyToEmail, _globalSettings.SiteName);
            smtpMessage.Subject = message.Subject;
            smtpMessage.SubjectEncoding = Encoding.UTF8;
            smtpMessage.BodyEncoding = Encoding.UTF8;
            smtpMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
            foreach(var address in message.ToEmails)
            {
                smtpMessage.To.Add(new MailAddress(address));
            }

            if(string.IsNullOrWhiteSpace(message.TextContent))
            {
                smtpMessage.IsBodyHtml = true;
                smtpMessage.Body = message.HtmlContent;
            }
            else
            {
                smtpMessage.Body = message.TextContent;
                var htmlView = AlternateView.CreateAlternateViewFromString(message.HtmlContent);
                htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                smtpMessage.AlternateViews.Add(htmlView);
            }

            client.SendCompleted += (sender, e) =>
            {
                if(e.Error != null)
                {
                    _logger.LogError(e.Error, "Mail send failed.");
                }

                if(e.Cancelled)
                {
                    _logger.LogWarning("Mail send canceled.");
                }

                smtpMessage.Dispose();
                client.Dispose();
            };

            client.SendAsync(smtpMessage, null);
            return Task.FromResult(0);
        }
    }
}
