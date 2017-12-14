using Consul;
using ConsulDemo.Helpers.ServiceDiscovery.Contracts;
using DnsClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net;

namespace ConsulDemo.Helpers.ServiceDiscovery.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterConsulClient(configuration)
                .RegisterDnsLookup()
                .RegisterServiceClient();

            return services;
        }

        private static IServiceCollection RegisterServiceClient(this IServiceCollection services)
        {
            services.AddScoped<IServiceClient, ServiceClient>();
            return services;
        }

        private static IServiceCollection RegisterDnsLookup(this IServiceCollection services)
        {
            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;

                var client = new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);

                if (serviceConfiguration.Consul.DnsEndpoint != null)
                {
                    client = new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIpEndPoint());
                }

                client.EnableAuditTrail = false;
                client.UseCache = true;
                client.MinimumCacheTimeout = TimeSpan.FromSeconds(1);
                return client;
            });

            return services;
        }

        private static IServiceCollection RegisterConsulClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ServiceDisvoveryOptions>(configuration.GetSection("ServiceDiscovery"));

            services.AddSingleton<IConsulClient>(p => new ConsulClient(config =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    config.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));

            return services;
        }
    }
}