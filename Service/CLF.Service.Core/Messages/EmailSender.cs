using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CLF.Common.Configuration;
using CLF.Common.Exceptions;
using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using Serilog;

namespace CLF.Service.Core.Messages
{
    public class EmailSender : IEmailSender
    {
        public  void SendEmail(EmailMessage emailMessage, bool isAsync = false)
        {
            if (isAsync)
            {
                SendAsync(emailMessage);
            }
            else
            {
                Send(emailMessage);
            }
        }

        public  void SendEmail(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null,

 List<string> bcc = null, IDictionary<string, string> headers = null)
        {
            try
            {
                var mailMessage = PrepareMailMessage(emailConfig, subject, body, toAddress, cc, bcc, headers);
                var smtpClient = PrepareSmtpClient(emailConfig);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
            catch (Exception ex)
            {
                //throw new ProcessException($"{ex.Message}");
            }
        }

        public  Task SendMailAsync(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null, List<string> bcc = null, IDictionary<string, string> headers = null)
        {
            try
            {
                var mailMessage = PrepareMailMessage(emailConfig, subject, body, toAddress, cc, bcc, headers);
                var smtpClient = PrepareSmtpClient(emailConfig);
                var result = smtpClient.SendAsync(mailMessage);
                smtpClient.Disconnect(true);
                return result;
            }

            catch (Exception ex)
            {
                return Task.CompletedTask;
            }
        }

        private  void Send(EmailMessage emailMessage)

        {
            var config = new EmailConfig();
            SendEmail(config, emailMessage.Subject, emailMessage.Body, emailMessage.To, emailMessage.CC, emailMessage.BCC, emailMessage.Headers);
        }

        private  Task SendAsync(EmailMessage emailMessage)
        {
            var config = new EmailConfig();
            return SendMailAsync(config, emailMessage.Subject, emailMessage.Body, emailMessage.To, emailMessage.CC, emailMessage.BCC, emailMessage.Headers);
        }

        private  SmtpClient PrepareSmtpClient(EmailConfig emailConfig)
        {
            var client = new SmtpClient();
            client.Connect(emailConfig.Host, emailConfig.Port);
            client.Authenticate(emailConfig.Email, emailConfig.Password);
            return client;
        }

        private static MimeMessage PrepareMailMessage(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null,

    List<string> bcc = null, IDictionary<string, string> headers = null)
        {
            if (toAddress == null || !toAddress.Any())

            {
                throw new Exception("收件人邮箱不能为空!");
            }

            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(emailConfig.DisplayName, emailConfig.Email));
            foreach (var toEmail in toAddress)
            {
                mailMessage.To.Add(new MailboxAddress(toEmail));
            }

            if (cc != null)
            {
                foreach (var ccEmail in cc)
                {
                    mailMessage.Cc.Add(new MailboxAddress(ccEmail));
                }
            }

            if (bcc != null)
            {
                foreach (var bccEmail in bcc)
                {
                    mailMessage.Bcc.Add(new MailboxAddress(bccEmail));
                }
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    mailMessage.Headers.Add(header.Key, header.Value);
                }
            }
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain") { Text = body };
            return mailMessage;
        }
    }
}
