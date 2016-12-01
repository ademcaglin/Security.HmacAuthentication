# Aspnet Core Hmac Authentication 

This library is just for illustration. Be careful when you plan to use for production.

Based on http://bitoftech.net/2014/12/15/secure-asp-net-web-api-using-api-key-authentication-hmac-authentication/ and https://github.com/johnhidey/Hmac .

# How to use

Copy files in Security.HmacAuthentication/src/HmacAuthentication/ folder into your project and write following code in Startup.cs

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            //...
            app.UseHmacAuthentication(new HmacAuthenticationOptions
            {
                 SecretKey = "<secret>",
                 AppId = "<app-id>",
                 AutomaticAuthenticate =true
           });
           //...
         }
