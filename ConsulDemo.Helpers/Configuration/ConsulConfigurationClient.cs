using Consul;
using ConsulDemo.Helpers.Configuration.Consul;
using ConsulDemo.Helpers.Configuration.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ConsulDemo.Helpers.Configuration
{
    internal sealed class ConsulConfigurationClient : IConsulConfigurationClient
    {
        private readonly IConsulClientFactory _consulClientFactory;
        private readonly object _lastIndexLock = new object();
        private readonly IConsulConfigurationSource _source;

        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
        private ulong _lastIndex;

        public ConsulConfigurationClient(IConsulClientFactory consulClientFactory, IConsulConfigurationSource source)
        {
            _consulClientFactory = consulClientFactory;
            _source = source;
        }

        public async Task<IConfigQueryResult> GetConfig()
        {
            var result = await GetKVPair();
            UpdateLastIndex(result);
            return new ConfigQueryResult(result);
        }

        public IChangeToken Watch(Action<ConsulWatchExceptionContext> onException)
        {
            Task.Run(() => PollForChanges(onException));
            return _reloadToken;
        }

        private async Task<QueryResult<KVPair>> GetKVPair(QueryOptions queryOptions = null)
        {
            using (var consulClient = _consulClientFactory.Create())
            {
                var result = await consulClient.KV.Get(_source.Key, queryOptions, _source.CancellationToken);
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NotFound:
                        return result;
                    default:
                        throw new Exception($"Error loading configuration from consul. Status code: {result.StatusCode}.");
                }
            }
        }

        private async Task PollForChanges(Action<ConsulWatchExceptionContext> onException)
        {
            while (!_source.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!await HasValueChanged()) continue;

                    var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
                    previousToken.OnReload();
                    return;
                }
                catch (Exception exception)
                {
                    var exceptionContext = new ConsulWatchExceptionContext(_source, exception);
                    onException?.Invoke(exceptionContext);
                }
            }
        }

        private async Task<bool> HasValueChanged()
        {
            QueryOptions queryOptions;
            lock (_lastIndexLock)
            {
                queryOptions = new QueryOptions { WaitIndex = _lastIndex };
            }

            var result = await GetKVPair(queryOptions);
            return result != null && UpdateLastIndex(result);
        }

        private bool UpdateLastIndex(QueryResult<KVPair> queryResult)
        {
            lock (_lastIndexLock)
            {
                if (queryResult.LastIndex > _lastIndex)
                {
                    _lastIndex = queryResult.LastIndex;
                    return true;
                }
            }

            return false;
        }
    }
}