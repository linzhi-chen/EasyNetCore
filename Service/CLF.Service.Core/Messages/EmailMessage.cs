using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Service.Core.Messages
{
  public  class EmailMessage
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 收件箱
        /// </summary>
        public List<string> To { get; set; }
        /// <summary>
        /// 抄送
        /// </summary>
        public List<string> CC { get; set; }
        /// <summary>
        /// 暗抄送
        /// </summary>
        public List<string> BCC { get; set; }
        /// <summary>
        /// 头部
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
    }
}
