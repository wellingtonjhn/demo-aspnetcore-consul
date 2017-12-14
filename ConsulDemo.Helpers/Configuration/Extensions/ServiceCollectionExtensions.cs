using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace ConsulDemo.Helpers.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static IConfigurationBuilder DefaultConfigurationBuilder(IHostingEnvironment env)
        {
            return new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
        }

        public static IServiceCollection AddConsulKeyValueStore(
            this IServiceCollection services, 
            string key, 
            IConfiguration configuration, 
            IHostingEnvironment env, 
            CancellationTokenSource cancellationTokenSource)
        {
            var builder = DefaultConfigurationBuilder(env);
            var consulHost = configuration.GetSection("ServiceDiscovery:Consul:HttpEndpoint").Value;
            var applicationName = configuration.GetSection("ServiceDiscovery:ServiceName").Value;
            var consulKey = $"{applicationName}/{env.EnvironmentName}/{key}";

            var configurationRoot = builder.AddConsul(consulKey, cancellationTokenSource.Token, options =>
            {
                options.ConsulConfigurationOptions = consul =>
                {
                    consul.Address = new Uri(consulHost);
                    options.Optional = true;
                    options.ReloadOnChange = true;
                    options.OnLoadException = exceptionContext => exceptionContext.Ignore = true;
                };
            }).Build();

            services.AddSingleton(configurationRoot);
            return services;
        }

    }
}