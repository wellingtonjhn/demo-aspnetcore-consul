using ConsulDemo.Helpers.Configuration.Contracts;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace ConsulDemo.Helpers.Configuration.Consul
{
    /// <summary>Provides client access for getting and watching config values in Consul.</summary>
    internal interface IConsulConfigurationClient
    {
        /// <summary>Gets the config from consul asynchronously.</summary>
        /// <returns>A Task containing the result of the query for the config.</returns>
        Task<IConfigQueryResult> GetConfig();

        /// <summary>Watches the config for changes.</summary>
        /// <param name="onException">An action to be invoked if an exception occurs during the watch.</param>
        /// <returns>An <see cref="IChangeToken"/> that will indicated when changes have occured.</returns>
        IChangeToken Watch(Action<ConsulWatchExceptionContext> onException);
    }
}