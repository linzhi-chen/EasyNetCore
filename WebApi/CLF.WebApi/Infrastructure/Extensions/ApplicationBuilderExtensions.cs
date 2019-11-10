using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLF.WebApi.Infrastructure.Extensions
{
    public static  class ApplicationBuilderExtensions
    {
        public static void UseAppSwagger(this IApplicationBuilder application)
        {
            application.UseSwagger();

            application.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = string.Empty;
            });
        }
    }
}
