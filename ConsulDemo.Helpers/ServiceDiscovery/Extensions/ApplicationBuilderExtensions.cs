using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsulDemo.Helpers.ServiceDiscovery.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static ILogger _logger;

        public static IApplicationBuilder UseConsulServiceRegistry(this IApplicationBuilder app, params string[] tags)
        {
            _logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("ServiceDiscoveryBuilder");

            var appLife = app.ApplicationServices.GetRequiredService<IApplicationLifetime>() ?? throw new ArgumentException("Missing dependency", nameof(IApplicationLifetime));
            var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ServiceDisvoveryOptions>>() ?? throw new ArgumentException("Missing dependency", nameof(IOptions<ServiceDisvoveryOptions>));
            var consul = app.ApplicationServices.GetRequiredService<IConsulClient>() ?? throw new ArgumentException("Missing dependency", nameof(IConsulClient));

            if (string.IsNullOrEmpty(serviceOptions.Value.ServiceName))
            {
                throw new ArgumentException("Service Name must be configured", nameof(serviceOptions.Value.ServiceName));
            }

            var addresses = GetEndpoints(app, serviceOptions);

            foreach (var address in addresses)
            {
                var serviceId = GetServiceId(serviceOptions, address);
                var serviceChecks = GetHealthCheckingConfiguration(serviceOptions, address, serviceId);
                ConfigureServiceRegistration(appLife, serviceOptions, consul, address, serviceId, serviceChecks, tags);
            }

            return app;
        }

        private static void ConfigureServiceRegistration(
            IApplicationLifetime appLife, 
            IOptions<ServiceDisvoveryOptions> serviceOptions, 
            IConsulClient consul, 
            Uri address, 
            string serviceId, 
            List<AgentServiceCheck> serviceChecks, 
            params string[] tags)
        {
            _logger.LogInformation($"Registering service {serviceId} for address {address}.");
            var registration = new AgentServiceRegistration
            {
                Checks = serviceChecks.ToArray(),
                Address = address.Host,
                ID = serviceId,
                Name = serviceOptions.Value.ServiceName,
                Port = address.Port,
                Tags = tags
            };

            consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

            appLife.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation($"Deregistering service {serviceId} for address {address}.");
                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            });
        }

        private static List<AgentServiceCheck> GetHealthCheckingConfiguration(IOptions<ServiceDisvoveryOptions> serviceOptions, Uri address, string serviceId)
        {
            var serviceChecks = new List<AgentServiceCheck>();

            if (string.IsNullOrEmpty(serviceOptions.Value.HealthCheck.HealthCheckEndpoint))
                return serviceChecks;

            var healthCheckUri = new Uri(address, serviceOptions.Value.HealthCheck.HealthCheckEndpoint).OriginalString;
            var deregisterCriticalServiceAfter = TimeSpan.FromMinutes(serviceOptions.Value.HealthCheck.DeregisterCriticalServiceAfterMinutes);
            var checkInterval = TimeSpan.FromSeconds(serviceOptions.Value.HealthCheck.CheckIntervalSeconds);

            serviceChecks.Add(new AgentServiceCheck
            {
                Status = HealthStatus.Passing,
                DeregisterCriticalServiceAfter = deregisterCriticalServiceAfter,
                Interval = checkInterval,
                HTTP = healthCheckUri
            });

            _logger.LogInformation($"[Consul] Healthcheck added for service {serviceId}, checking {healthCheckUri}.");

            return serviceChecks;
        }

        private static IEnumerable<Uri> GetEndpoints(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions)
        {
            IEnumerable<Uri> addresses = null;
            if (serviceOptions.Value.Endpoints != null && serviceOptions.Value.Endpoints.Length > 0)
            {
                _logger.LogInformation($"[Consul] Using {serviceOptions.Value.Endpoints.Length} configured endpoints for service registration.");
                addresses = serviceOptions.Value.Endpoints.Select(p => new Uri(p));
            }
            else
            {
                _logger.LogInformation("[Consul] Trying to use server.Features to figure out the service endpoints for service registration.");

                if (app.Properties["server.Features"] is FeatureCollection features)
                {
                    addresses = features.Get<IServerAddressesFeature>()
                        .Addresses
                        .Select(p => new Uri(p)).ToArray();
                }
            }

            _logger.LogInformation($"[Consul] Found {addresses.Count()} endpoints: {string.Join(",", addresses.Select(p => p.OriginalString))}.");
            return addresses;
        }

        private static string GetServiceId(IOptions<ServiceDisvoveryOptions> serviceOptions, Uri address) 
            => $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";
    }
}