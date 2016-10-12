using Microsoft.AspNetCore.Authentication.Hmac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HmacTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
            services.AddMemoryCache();
        }
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            app.Map("/validate", builder =>
            {
                builder.UseHmacAuthentication(new HmacAuthenticationOptions
                {
                    SecretKey = "abc670d15a584f4baf0ba48455d3b155",
                    AppId = "jDEf7bMcJVFnqrPd599aSIbhC0IasxLBpGAJeW3Fzh4=",
                    AutomaticAuthenticate =true
                });
                builder.Run(async (context) =>
                {
                    //context.User = await context.Authentication.AuthenticateAsync(HmacAuthenticationDefaults.AuthenticationScheme);
                    //it should be True
                    await context.Response.WriteAsync(context.User.Identity.IsAuthenticated.ToString());
                });
            });

            app.Map("/replayattack", builder =>
            {
                builder.UseHmacAuthentication(new HmacAuthenticationOptions
                {
                    SecretKey = "abc670d15a584f4baf0ba48455d3b155",
                    AppId = "jDEf7bMcJVFnqrPd599aSIbhC0IasxLBpGAJeW3Fzh4=",
                    AutomaticAuthenticate = true,
                    MaxRequestAgeInSeconds = 300
                });
                builder.Run(async (context) =>
                {
                    //context.User = await context.Authentication.AuthenticateAsync(HmacAuthenticationDefaults.AuthenticationScheme);
                    //it should be True
                    await context.Response.WriteAsync(context.User.Identity.IsAuthenticated.ToString());
                });
            });
        }
    }
}
