using Consul;
using ConsulDemo.Helpers.Configuration.Contracts;
using System.Net;

namespace ConsulDemo.Helpers.Configuration
{
    internal sealed class ConfigQueryResult : IConfigQueryResult
    {
        public bool Exists { get; }
        public byte[] Value { get; }

        public ConfigQueryResult(QueryResult<KVPair> kvPairQueryResult)
        {
            Exists = kvPairQueryResult?.StatusCode != HttpStatusCode.NotFound
                && kvPairQueryResult?.Response?.Value != null
                && kvPairQueryResult?.Response?.Value.Length != 0;
            Value = kvPairQueryResult?.Response?.Value;
        }
    }
}