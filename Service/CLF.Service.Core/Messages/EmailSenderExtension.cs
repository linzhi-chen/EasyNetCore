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
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailSender"></param>
        /// <param name="emailMessage"></param>
        /// <param name="isAsync"></param>
        public static void SendEmail(this IEmailSender emailSender, EmailMessage emailMessage,bool isAsync=false)
        {
            if (isAsync)
            {
                SendAsync(emailSender, emailMessage);
            }
            else
            {
                Send(emailSender, emailMessage);
            }
        }

        private static void Send(IEmailSender emailSender, EmailMessage emailMessage)
        {
            var config = EngineContext.Current.Resolve<EmailConfig>();
            emailSender.SendEmail(config, emailMessage.Subject, emailMessage.Body, emailMessage.To, emailMessage.CC, emailMessage.BCC, emailMessage.Headers);
        }

        private static Task SendAsync(IEmailSender emailSender, EmailMessage emailMessage)
        {
            var config = EngineContext.Current.Resolve<EmailConfig>();
            return emailSender.SendMailAsync(config, emailMessage.Subject, emailMessage.Body, emailMessage.To, emailMessage.CC, emailMessage.BCC, emailMessage.Headers);
        }
    }
}
