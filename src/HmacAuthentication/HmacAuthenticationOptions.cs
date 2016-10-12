using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Hmac;

namespace Microsoft.AspNetCore.Builder
{
    public class HmacAuthenticationOptions : AuthenticationOptions
    {
        public ulong MaxRequestAgeInSeconds { get; set; }

        public string AppId { get; set; }

        public string SecretKey { get; set; }      

        public HmacAuthenticationOptions()
        {
            MaxRequestAgeInSeconds = HmacAuthenticationDefaults.MaxRequestAgeInSeconds;
            AuthenticationScheme = HmacAuthenticationDefaults.AuthenticationScheme;
        }
    }
}
