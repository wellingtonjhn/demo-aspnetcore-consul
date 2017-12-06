using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using ConfigurationBuilder = ConsulDemo.Extensions.ConfigurationBuilder;

namespace ConsulDemo
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private readonly CancellationTokenSource _consulCancellationSource = new CancellationTokenSource();

        public Startup(IHostingEnvironment env)
        {
            var consulKey = $"{env.ApplicationName}/{env.EnvironmentName}/appsettings";
            Configuration = ConfigurationBuilder.ConfigureConsulProvider(consulKey, env, _consulCancellationSource);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(Configuration)
                .AddLogging()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory
                .AddConsole(LogLevel.Debug)
                .AddDebug(LogLevel.Debug);

            app.UseMvc();
        }
    }
}
