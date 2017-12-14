using Consul;

namespace ConsulDemo.Helpers.Configuration.Contracts
{
    /// <summary>A factory responsible for creating <see cref="IConsulClient"/> objects.</summary>
    internal interface IConsulClientFactory
    {
        /// <summary>Creates a new instance of an <see cref="IConsulClient"/>.</summary>
        /// <returns>A new <see cref="IConsulClient"/>.</returns>
        IConsulClient Create();
    }
}