using ConsulDemo.Helpers.Configuration.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace ConsulDemo.Helpers.Configuration.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder builder, string key, CancellationToken cancellationToken)
        {
            return builder.AddConsul(key, cancellationToken, options => { });
        }

        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder builder, string key, CancellationToken cancellationToken, Action<IConsulConfigurationSource> options)
        {
            var consulConfigSource = new ConsulConfigurationSource(key, cancellationToken);
            options(consulConfigSource);
            return builder.Add(consulConfigSource);
        }
    }
}