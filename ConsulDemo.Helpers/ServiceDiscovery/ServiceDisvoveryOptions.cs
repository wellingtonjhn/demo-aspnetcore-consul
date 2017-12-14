using System.Net;

namespace ConsulDemo.Helpers.ServiceDiscovery
{
    public class ServiceDisvoveryOptions
    {
        public string ServiceName { get; set; }

        public ConsulOptions Consul { get; set; }

        public HealthCheck HealthCheck { get; set; }

        public string[] Endpoints { get; set; }
    }

    public class HealthCheck
    {
        public string HealthCheckEndpoint { get; set; }
        public int DeregisterCriticalServiceAfterMinutes { get; set; }
        public int CheckIntervalSeconds { get; set; }
    }

    public class ConsulOptions
    {
        public string HttpEndpoint { get; set; }

        public DnsEndpoint DnsEndpoint { get; set; }
    }

    public class DnsEndpoint
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public IPEndPoint ToIpEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Address), Port);
        }
    }
}