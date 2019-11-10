using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CLF.WebApi.Infrastructure.Extensions
{
    public static  class ServiceCollectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "CLF.WebApi",
                    Description = "webapi接口文档",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "chenlinfei",
                        Email = string.Empty,
                        Url = "https://github.com/linfei-chen/simplenetcore"
                    },
                    License = new License
                    {
                        Name = "chenlinfei",
                        Url = "https://github.com/linfei-chen/simplenetcore"
                    }
                });

                //请求时头部加入token
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });

                //设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(CLF.WebApi.Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "Swagger.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
