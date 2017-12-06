using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using Winton.Extensions.Configuration.Consul;

namespace ConsulDemo.Extensions
{
    public static class ConfigurationBuilder
    {
        public static IConfigurationBuilder GetConfigurationBuilder(string jsonFile, IHostingEnvironment env)
        {
            var filename = Path.GetFileNameWithoutExtension(jsonFile);

            return new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"{filename}.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{filename}.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
        }

        public static IConfigurationRoot ConfigureJsonFileProvider(string jsonFile, IHostingEnvironment env)
        {
            return GetConfigurationBuilder(jsonFile, env)
                .Build();
        }

        public static IConfigurationRoot ConfigureConsulProvider(string key, IHostingEnvironment env, CancellationTokenSource cts)
        {
            const string jsonFile = "appsettings.json";

            var builder = GetConfigurationBuilder(jsonFile, env);
            var consulHost = builder.Build().GetSection("Consul:Host").Value;

            var configurationRoot = builder.AddConsul(
                    key,
                    cts.Token,
                    options =>
                    {
                        options.ConsulConfigurationOptions = consul =>
                        {
                            consul.Address = new Uri(consulHost);
                        };
                        options.Optional = true;
                        options.ReloadOnChange = true;
                        options.OnLoadException = (exceptionContext) =>
                        {
                            exceptionContext.Ignore = true;
                        };
                    })
                .Build();

            return configurationRoot;
        }
    }
}