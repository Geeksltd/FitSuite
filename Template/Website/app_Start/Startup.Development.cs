using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Olive;
using Olive.Security;

namespace Website
{
    public class StartupDevelopment : Startup
    {
        public StartupDevelopment(IWebHostEnvironment env, IConfiguration config, ILoggerFactory factory) : base(env, config, factory)
        {
        }

        protected override void ConfigureDataProtectionProvider(Microsoft.AspNetCore.Authentication.Google.GoogleOptions config)
        {
            var key = Config.Get("Authentication:CookieDataProtectorKey");
            config.DataProtectionProvider = new SymmetricKeyDataProtector("AuthCookies", key);
        }
    }
}