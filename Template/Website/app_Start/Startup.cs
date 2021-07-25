using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Olive;
using Olive.Microservices.Hub;

namespace Website
{
    public abstract class Startup : HubStartup<TaskManager>
    {
        protected Startup(IWebHostEnvironment env, IConfiguration config, ILoggerFactory factory) : base(env, config, factory)
        {
        }

        protected override void ConfigureDataProtectionProvider(GoogleOptions config)
        {
        }

        protected override void ConfigureAuthentication(AuthenticationBuilder auth)
        {
            base.ConfigureAuthentication(auth);
            //AddGoogle(auth);
        }

        protected virtual void AddGoogle(AuthenticationBuilder auth)
        {
            auth.AddGoogle(config =>
            {
                config.ClientId = Config.Get("Authentication:Google:ClientId");
                config.ClientSecret = Config.Get("Authentication:Google:ClientSecret");

                ConfigureDataProtectionProvider(config);
            });
        }

        public override async Task OnStartUpAsync(IApplicationBuilder app)
        {
            await base.OnStartUpAsync(app);
            
        }
    }
}