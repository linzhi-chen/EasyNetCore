using CLF.Common.Configuration;
using CLF.Common.Http;
using CLF.Common.Infrastructure;
using CLF.DataAccess.Account;
using CLF.Model.Account;
using CLF.Web.Framework.Identity.Providers;
using CLF.Web.Framework.Mvc.Filters;
using EasyCaching.Core;
using EasyCaching.InMemory;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Web.Framework.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            services.AddHttpContextAccessor();

            var emailConfig = services.ConfigureStartupConfig<EmailConfig>(configuration.GetSection("Email"));
            var appConfig = services.ConfigureStartupConfig<AppConfig>(configuration.GetSection("App"));

            var engine = EngineContext.Create();
            var serviceProvider = engine.ConfigureServices(services, configuration, appConfig);

            services.AddSerilog();

            return serviceProvider;
        }

        /// <summary>
        /// 配置数据库上下文
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContext<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : DbContext
        {
            var appConfig = services.ConfigureStartupConfig<AppConfig>(configuration.GetSection("App"));
            services.AddDbContextPool<TDbContext>(optionsBuilder =>
            {
                if (appConfig.MySqlEnabled)
                    optionsBuilder.UseMySql(appConfig.MySqlConnectionString);
                else
                    optionsBuilder.UseSqlServer(appConfig.SqlServerConnectionString);
            });
        }

        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        public static IMvcBuilder AddAppMvc(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc(options =>
            {
                //options.Filters.Add<HttpExceptionFilter>(); //全局异常过滤器
            });

            mvcBuilder.AddMvcOptions(options => options.EnableEndpointRouting = false);

            mvcBuilder.AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; //忽略循环引用
                options.SerializerSettings.ContractResolver = new DefaultContractResolver(); //不使用驼峰样式的key
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            mvcBuilder.AddControllersAsServices();
     
            return mvcBuilder;
        }

        public static IdentityBuilder AddAppIdentity<TUser, TRole>(this IServiceCollection services) where TUser : IdentityUser where TRole : IdentityRole
        {
            var identityBuilder = services.AddIdentity<TUser, TRole>(options =>
              {
                  options.Tokens.ProviderMap.Add("CustomEmailConfirmation", new TokenProviderDescriptor(typeof(CustomEmailConfirmationTokenProvider<TUser>)));
                  options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                  //用户名验证
                  options.User.RequireUniqueEmail = true;
                 
                  options.SignIn.RequireConfirmedEmail = true;
                 
                  //配置用户锁定
                  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                  options.Lockout.MaxFailedAccessAttempts = 10;

                  //配置密码
                  options.Password.RequiredLength = 6;
                  options.Password.RequireDigit = false;
                  options.Password.RequireLowercase = false;
                  options.Password.RequireUppercase = false;
                  options.Password.RequireNonAlphanumeric = false;
              })
               //.AddDefaultTokenProviders()
               .AddEntityFrameworkStores<AccountContext>();

            return identityBuilder;
        }

        public static void AddEasyCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(option =>
            {
                option.UseInMemory("clfnetcore_memory_cache");
            });
        }

        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                //options.Cookie.IsEssential = true;
            });
        }

        public static void AddSerilog(this IServiceCollection services)
        {
            var config = EngineContext.Current.Resolve<AppConfig>();

            var logEventLevel = GetLogEventLevel(config.LogEventLevel);
            var rollingInterval = GetRollingInterval(config.RollingInterval);

            var serilog = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.File(config.LogFilePath, logEventLevel, rollingInterval: rollingInterval)
                 .CreateLogger();

            //启动时创建logger对象，静态方法，后面所有地方通过类似Log.Debug()方式调用，无需再创建logger对象
            Log.Logger = serilog;

            //注入.net core 日志框架
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            loggerFactory.AddSerilog(serilog);

            //系统遇到问题继续把内存中的日志信息输出到日志文件
            IApplicationLifetime appLifetime = EngineContext.Current.Resolve<IApplicationLifetime>();
            if (appLifetime != null)
            {
                appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
            }

            LogEventLevel GetLogEventLevel(string logLevel)
            {
                switch (logLevel)
                {
                    case "Verbose":
                        return LogEventLevel.Verbose;
                    case "Debug":
                        return LogEventLevel.Debug;
                    case "Information":
                        return LogEventLevel.Information;
                    case "Warning":
                        return LogEventLevel.Warning;
                    case "Error":
                        return LogEventLevel.Error;
                    case "Fatal":
                        return LogEventLevel.Fatal;
                    default:
                        return LogEventLevel.Error;
                }
            }

            RollingInterval GetRollingInterval(string interval)
            {
                switch (interval)
                {
                    case "Infinite":
                        return RollingInterval.Infinite;
                    case "Year":
                        return RollingInterval.Year;
                    case "Day":
                        return RollingInterval.Day;
                    case "Month":
                        return RollingInterval.Month;
                    case "Hour":
                        return RollingInterval.Hour;
                    case "Minute":
                        return RollingInterval.Minute;
                    default:
                        return RollingInterval.Hour;
                }
            }
        }

        /// <summary>
        /// 添加认证授权
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var appConfig = services.ConfigureStartupConfig<AppConfig>(configuration.GetSection("App"));

            // 默认cookie认证
            var authenticationBuilder = services.AddAuthentication(options =>
              {
                  if (appConfig.JwtAuthenticationEnabled || appConfig.OAuthAuthenticationEnabled)
                  {
                      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //绑定[Authorize]，否则报401
                      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//认证未通过防止重定向到登录页，而是显示401
                  }
                  else
                  {
                      options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                      options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                  }
              })
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
             {
                 options.LoginPath = new PathString("/account/login");
                 options.Cookie.Expiration = TimeSpan.FromHours(appConfig.DefaultCookieExpiredHours);
             });

            //配置jwt
            if (appConfig.JwtAuthenticationEnabled)
            {
                var jwtConfig = services.ConfigureStartupConfig<JwtConfig>(configuration.GetSection("Jwt"));
                var oAuthConfig = services.ConfigureStartupConfig<OAuthConfig>(configuration.GetSection("OAuth"));
                AddJwtConfiguration(authenticationBuilder, jwtConfig);
            }

            //配置oauth
            if(appConfig.OAuthAuthenticationEnabled)
            {
                var oAuthConfig = services.ConfigureStartupConfig<OAuthConfig>(configuration.GetSection("OAuth"));
                AddOAuthConfiguration(authenticationBuilder, oAuthConfig);
            }
        }

        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var config = new TConfig();

            //绑定配置文件到对象
            configuration.Bind(config);

            //注册TConfig对象，后续通过EngineContext.Current.Resolve<TConfig>()即可获取到相关配置
            services.AddSingleton(config);

            return config;
        }

        private static void AddJwtConfiguration(AuthenticationBuilder authenticationBuilder, JwtConfig jwtConfig)
        {
            authenticationBuilder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //token失效起始时间间隔，设置0，从生成token开始算失效时间，不设置默认5分钟
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        /// <summary>
        /// 配置OAuth认证
        /// </summary>
        /// <param name="authenticationBuilder"></param>
        /// <param name="oAuthConfig"></param>
        private static void AddOAuthConfiguration(AuthenticationBuilder authenticationBuilder, OAuthConfig oAuthConfig)
        {
            authenticationBuilder.AddJwtBearer(options =>
            {
                options.Authority = oAuthConfig.Authority;
                options.Audience = oAuthConfig.Audience;
                options.RequireHttpsMetadata = oAuthConfig.HttpsRequired;
            });
        }
    }
}
