using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Common.Configuration
{
   public class EmailConfig
    {
        /// <summary>
        /// 默认发送邮件名
        /// </summary>
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }
        
        public string Password { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// 是否是异步发送邮件
        /// </summary>
        public bool SendEmailAsync { get; set; }
    }
}
