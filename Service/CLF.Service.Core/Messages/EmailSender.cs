using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CLF.Common.Configuration;
using CLF.Common.Exceptions;
using Newtonsoft.Json;
using Serilog;

namespace CLF.Service.Core.Messages
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null,
         List<string> bcc = null, IDictionary<string, string> headers = null)
        {
            try
            {
                var mailMessage = PrepareMailMessage(emailConfig, subject, body, toAddress, cc, bcc, headers);
                var smtpClient = PrepareSmtpClient(emailConfig);
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public Task SendMailAsync(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null, List<string> bcc = null, IDictionary<string, string> headers = null)
        {
            try
            {
                var mailMessage = PrepareMailMessage(emailConfig, subject, body, toAddress, cc, bcc, headers);
                var smtpClient = PrepareSmtpClient(emailConfig);
                return smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Task.CompletedTask;
            }
        }

        private SmtpClient PrepareSmtpClient(EmailConfig emailConfig)
        {
            var smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = emailConfig.UseDefaultCredentials;
            smtpClient.Host = emailConfig.Host;
            smtpClient.Port = emailConfig.Port;
            smtpClient.EnableSsl = emailConfig.EnableSsl;
            smtpClient.Credentials = emailConfig.UseDefaultCredentials ?
                CredentialCache.DefaultNetworkCredentials :
                new NetworkCredential(emailConfig.UserName, emailConfig.Password);
            return smtpClient;
        }

        private MailMessage PrepareMailMessage(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null,
    List<string> bcc = null, IDictionary<string, string> headers = null)
        {
            if (toAddress == null || !toAddress.Any())
            {
                throw new ComponentException("收件人邮箱不能为空!");
            }

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailConfig.Email, emailConfig.DisplayName);
            foreach (var toEmail in toAddress)
            {
                mailMessage.To.Add(toEmail);
            }

            if (cc != null)
            {
                foreach (var ccEmail in cc)
                {
                    mailMessage.CC.Add(ccEmail);
                }
            }

            if (bcc != null)
            {
                foreach (var bccEmail in bcc)
                {
                    mailMessage.Bcc.Add(bccEmail);
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
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }
    }
}
