using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Hmac
{
    internal class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        private readonly IMemoryCache _memoryCache;

        public HmacAuthenticationHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers["authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.Skip();
            }
            var valid = Validate(Request);

            if (valid)
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity("HMAC"));
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Authentication failed");

        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            return base.HandleUnauthorizedAsync(context);
        }

        private bool Validate(HttpRequest request)
        {
            var header = request.Headers["authorization"];
            var authenticationHeader = AuthenticationHeaderValue.Parse(header);
            if (Options.AuthenticationScheme.Equals(authenticationHeader.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var rawAuthenticationHeader = authenticationHeader.Parameter;
                var authenticationHeaderArray = GetAuthenticationValues(rawAuthenticationHeader);

                if (authenticationHeaderArray != null)
                {
                    var AppId = authenticationHeaderArray[0];
                    var incomingBase64Signature = authenticationHeaderArray[1];
                    var nonce = authenticationHeaderArray[2];
                    var requestTimeStamp = authenticationHeaderArray[3];

                    return isValidRequest(request, AppId, incomingBase64Signature, nonce, requestTimeStamp);
                }
            }

            return false;
        }

        private bool isValidRequest(HttpRequest req, string AppId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            string requestContentBase64String = "";
            var absoluteUri = string.Concat(
                        req.Scheme,
                        "://",
                        req.Host.ToUriComponent(),
                        req.PathBase.ToUriComponent(),
                        req.Path.ToUriComponent(),
                        req.QueryString.ToUriComponent());
            string requestUri = WebUtility.UrlEncode(absoluteUri);
            string requestHttpMethod = req.Method;

            if (Options.AppId != AppId)
            {
                return false;
            }

            var sharedKey = Options.SecretKey;

            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            byte[] hash = ComputeHash(req.Body);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}{5}", AppId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }

        }

        private string[] GetAuthenticationValues(string rawAuthenticationHeader)
        {
            var credArray = rawAuthenticationHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }

        private bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            var nonceInMemory = _memoryCache.Get(nonce);
            if ( nonceInMemory != null)
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);
            var diff = (serverTotalSeconds - requestTotalSeconds);

            if (diff > Options.MaxRequestAgeInSeconds)
            {
                return true;
            }
            _memoryCache.Set(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(Options.MaxRequestAgeInSeconds));
            return false;
        }

        private byte[] ComputeHash(Stream body)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = ReadFully(body);
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
