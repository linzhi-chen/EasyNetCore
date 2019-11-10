using CLF.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Service.Core.Messages
{
    public interface IEmailSender
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailConfig"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="toAddress"></param>
        /// <param name="cc">暗抄送</param>
        /// <param name="bcc">抄送</param>
        /// <param name="headers"></param>
        void SendEmail(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null,
            List<string> bcc = null, IDictionary<string, string> headers = null);

        Task SendMailAsync(EmailConfig emailConfig, string subject, string body, List<string> toAddress, List<string> cc = null,
       List<string> bcc = null, IDictionary<string, string> headers = null);
    }
}
