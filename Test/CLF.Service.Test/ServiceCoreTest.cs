using CLF.Common.Infrastructure;
using CLF.Service.Core.Messages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Service.Test
{
    [TestFixture]
    public class ServiceCoreTest:ServiceTestBase
    {
        [Test]
        public void SendEmailTest()
        {
            IEmailSender emailSender=EngineContext.Current.Resolve<IEmailSender>();
            EmailMessage emailMessage = new EmailMessage
            {
                Subject = "test email",
                Body = "hello,boy",
                To= new List<string> { "172678033@qq.com" }
            };
            emailSender.Send(emailMessage);
        }
    }
}
