using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Hmac
{
    public class HmacAuthenticationMiddleware : AuthenticationMiddleware<HmacAuthenticationOptions>
    {
        private readonly IMemoryCache _memoryCache;
        public HmacAuthenticationMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IOptions<HmacAuthenticationOptions> options,
            IMemoryCache memoryCache)
            : base(next, options, loggerFactory, encoder)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _memoryCache = memoryCache;
        }
        protected override AuthenticationHandler<HmacAuthenticationOptions> CreateHandler()
        {
            return new HmacAuthenticationHandler(_memoryCache);
        }
    }
}
