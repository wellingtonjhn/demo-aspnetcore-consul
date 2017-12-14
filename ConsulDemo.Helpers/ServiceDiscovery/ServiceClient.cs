using Consul;
using ConsulDemo.Helpers.ServiceDiscovery.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulDemo.Helpers.ServiceDiscovery
{
    public class ServiceClient : IServiceClient
    {
        private readonly IConsulClient _consul;

        public ServiceClient(IConsulClient consul)
        {
            _consul = consul;
        }

        public async Task<ServiceInstance> GetServiceInstance(string service)
        {
            var queryResult = await _consul.Catalog.Service(service);
            var instance = queryResult.Response.FirstOrDefault();

            return new ServiceInstance
            {
                ServicePort = instance.ServicePort,
                ServiceAddress = instance.ServiceAddress,
                ServiceName = instance.ServiceName,
                Address = instance.Address,
                Node = instance.Node,
                ServiceId = instance.ServiceID,
                ServiceEndpoint = $"{instance.ServiceAddress}:{instance.ServicePort}",
                ServiceTags = instance.ServiceTags
            };
        }
    }
}