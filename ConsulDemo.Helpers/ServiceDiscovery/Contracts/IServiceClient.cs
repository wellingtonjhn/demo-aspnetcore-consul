using System.Threading.Tasks;

namespace ConsulDemo.Helpers.ServiceDiscovery.Contracts
{
    public interface IServiceClient
    {
        Task<ServiceInstance> GetServiceInstance(string service);
    }
}