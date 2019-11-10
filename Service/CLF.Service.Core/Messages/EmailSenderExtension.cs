using CLF.Common.Configuration;
using CLF.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Service.Core.Messages
{
  public static   class EmailSenderExtension
    {
        public static void Send( this IEmailSender emailSender,EmailMessage emailMessage)
        {
            var config = EngineContext.Current.Resolve<EmailConfig>();
            emailSender.SendEmail(config, emailMessage.Subject, emailMessage.Body, emailMessage.To, emailMessage.CC, emailMessage.BCC,emailMessage.Headers);
        }

        public static Task SendAsync(this IEmailSender emailSender, EmailMessage emailMessage)
        {
            var config = EngineContext.Current.Resolve<EmailConfig>();
            return emailSender.SendMailAsync(config, emailMessage.Subject, emailMessage.Body, emailMessage.To, emailMessage.CC, emailMessage.BCC, emailMessage.Headers);
        }
    }
}
