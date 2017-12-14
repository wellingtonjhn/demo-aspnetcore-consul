using ConsulDemo.Helpers.Configuration.Consul;
using ConsulDemo.Helpers.Configuration.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsulDemo.Helpers.Configuration
{
    internal sealed class ConsulConfigurationProvider : ConfigurationProvider
    {
        private readonly IConsulConfigurationClient _consulConfigClient;
        private readonly IConsulConfigurationSource _source;

        public ConsulConfigurationProvider(IConsulConfigurationSource source, IConsulConfigurationClient consulConfigClient)
        {
            if (source.Parser == null)
            {
                throw new ArgumentNullException(nameof(source.Parser));
            }

            _consulConfigClient = consulConfigClient;
            _source = source;

            if (source.ReloadOnChange)
            {
                ChangeToken.OnChange(
                    () => _consulConfigClient.Watch(_source.OnWatchException),
                    async () =>
                    {
                        await DoLoad(reloading: true);
                        OnReload();
                    });
            }
        }

        public override void Load()
        {
            try
            {
                DoLoad(reloading: false).Wait();
            }
            catch (AggregateException aggregateException)
            {
                throw aggregateException.InnerException;
            }
        }

        private async Task DoLoad(bool reloading)
        {
            try
            {
                var configQueryResult = await _consulConfigClient.GetConfig();
                if (!configQueryResult.Exists && !_source.Optional)
                {
                    if (!reloading)
                    {
                        throw new Exception($"The configuration for key {_source.Key} was not found and is not optional.");
                    }
                    return;
                }

                LoadIntoMemory(configQueryResult);
            }
            catch (Exception exception)
            {
                HandleLoadException(exception);
            }
        }

        private void LoadIntoMemory(IConfigQueryResult configQueryResult)
        {
            if (!configQueryResult.Exists)
            {
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            using (var configStream = new MemoryStream(configQueryResult.Value))
            {
                var parsedData = _source.Parser.Parse(configStream);
                Data = new Dictionary<string, string>(parsedData, StringComparer.OrdinalIgnoreCase);
            }
        }

        private void HandleLoadException(Exception exception)
        {
            var exceptionContext = new ConsulLoadExceptionContext(_source, exception);
            _source.OnLoadException?.Invoke(exceptionContext);

            if (!exceptionContext.Ignore)
            {
                throw exception;
            }
        }
    }
}