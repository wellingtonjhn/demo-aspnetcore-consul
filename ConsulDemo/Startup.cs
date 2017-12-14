using ConsulDemo.Helpers.Configuration.Extensions;
using ConsulDemo.Helpers.ServiceDiscovery.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace ConsulDemo.Api
{
    public class Startup
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddConsul(Configuration)
                .AddConsulKeyValueStore("appsettings", Configuration, Environment, _cts)
                .AddLogging()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();

            app.UseConsulServiceRegistry("aspnet", "service", "demo", "consul")
                .UseMvc();
        }
    }
}
