using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLF.Common.Configuration;
using CLF.Web.Framework.Infrastructure.Extensions;
using CLF.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CLF.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();
            var serviceProvider = services.ConfigureApplicationServices(_configuration, _hostingEnvironment);
            return serviceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAppSwagger();
            app.ConfigureRequestPipeline();
        }
    }
}
