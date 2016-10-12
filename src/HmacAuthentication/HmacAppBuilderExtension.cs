using Microsoft.AspNetCore.Authentication.Hmac;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class HmacAppBuilderExtension
    {
        public static IApplicationBuilder UseHmacAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<HmacAuthenticationMiddleware>();
        }

        public static IApplicationBuilder UseHmacAuthentication(this IApplicationBuilder app, HmacAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<HmacAuthenticationMiddleware>(Options.Create(options));
        }
    }
}
