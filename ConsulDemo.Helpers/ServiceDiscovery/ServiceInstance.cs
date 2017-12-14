namespace ConsulDemo.Helpers.ServiceDiscovery
{
    public class ServiceInstance
    {
        public string Node { get; set; }
        public string Address { get; set; }
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public int ServicePort { get; set; }
        public string ServiceEndpoint { get; set; }
        public string[] ServiceTags { get; set; }
    }
}