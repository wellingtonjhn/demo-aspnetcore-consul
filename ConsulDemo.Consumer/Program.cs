using ConsulDemo.Helpers.ServiceDiscovery.Contracts;
using ConsulDemo.Helpers.ServiceDiscovery.Extensions;
using DnsClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;

namespace ConsulDemo.Consumer
{
    class Program
    {
        private readonly IDnsQuery _dns;
        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            InitializeProgram();

            var serviceClient = _serviceProvider.GetService<IServiceClient>();
            var service = serviceClient.GetServiceInstance("ConsulDemoService").Result;

            Console.WriteLine("\nDiscovered service: ");
            Console.WriteLine(JsonConvert.SerializeObject(service, Formatting.Indented));

            using (var client = new HttpClient())
            {
                var serviceResult = client.GetStringAsync($"http://{service.ServiceEndpoint}/config/name").Result;

                Console.WriteLine($"\n\nCalling service {service.ServiceName}");
                Console.WriteLine($"Service call result: {serviceResult}");
            }

            Console.ReadKey();
        }

        private static void InitializeProgram()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _serviceProvider = new ServiceCollection()
                .AddConsul(configuration)
                .BuildServiceProvider();
        }
    }
}
