using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Hmac
{
    public class HmacAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for HmacAuthenticationOptions.AuthenticationScheme
        /// </summary>
        public const string AuthenticationScheme = "Hmac";

        public const int MaxRequestAgeInSeconds = 300;
    }
}
