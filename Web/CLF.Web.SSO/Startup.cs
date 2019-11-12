using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLF.Common.Configuration;
using CLF.DataAccess.Account;
using CLF.Model.Account;
using CLF.Web.Framework.Identity.Providers;
using CLF.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CLF.Web.SSO
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AccountContext>(Configuration);
            services.AddEntityFrameworkSqlServer();
            services.AddEntityFrameworkProxies();

            services.AddAppIdentity<AspNetUsers, AspNetRoles>();
            services.AddScoped<CustomEmailConfirmationTokenProvider<AspNetUsers>>();

            //dotnet new  is4ui --force
            AddAppIdentityServer(services, Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }

        /// <summary>
        /// identityserver4配置（OAuth）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="testUsers"></param>
        public void AddAppIdentityServer(IServiceCollection services, IConfiguration configuration)
        {
            var config = services.ConfigureStartupConfig<OAuthConfig>(configuration.GetSection("OAuth"));
            var identityServerBuilder = services.AddIdentityServer()
               .AddDeveloperSigningCredential()
               .AddInMemoryIdentityResources(config.GetIdentityResources())
               .AddInMemoryApiResources(config.GetApis())
               .AddInMemoryClients(config.GetClients());

            if (config.PasswordAuthorizationEnabled)
                identityServerBuilder.AddTestUsers(GetIdentityTestUsers(services));
        }

        /// <summary>
        /// 用户量多可以缓存，项目启动后新用户如何验证值得思考
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private List<IdentityServer4.Test.TestUser> GetIdentityTestUsers(IServiceCollection services)
        {
            var userManager = services.BuildServiceProvider().GetService<UserManager<AspNetUsers>>();
            var testUsers = new List<IdentityServer4.Test.TestUser>();
            foreach (var user in userManager.Users)
            {
                testUsers.Add(new IdentityServer4.Test.TestUser
                {
                    SubjectId = user.Id,
                    Username = user.UserName,
                    Password = user.PasswordHash
                });
            }
            return testUsers;
        }
    }
}
