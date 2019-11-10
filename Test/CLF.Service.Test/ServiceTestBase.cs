using CLF.Common.Infrastructure;
using CLF.DataAccess.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using CLF.Web.Framework.Infrastructure.Extensions;
using CLF.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.TestHost;

namespace CLF.Service.Test
{
    [TestFixture]
    public  class ServiceTestBase
    {
        [SetUp]
        public void UseStartup()
        {
            var builder = new WebHostBuilder()
                   .ConfigureAppConfiguration(config =>
                   {
                       config.SetBasePath(Directory.GetCurrentDirectory());
                       config.AddJsonFile("appsettings.json");
                   })
                   .UseStartup<CLF.Web.Mvc.Startup>();

            var server = new TestServer(builder);
        }
    }
}
