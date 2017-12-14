using Consul;
using ConsulDemo.Helpers.Configuration.Parsers;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading;

namespace ConsulDemo.Helpers.Configuration.Contracts
{
    /// <summary>
    /// An <see cref="IConfigurationSource"/> for the ConsulConfigurationProvider
    /// </summary>
    public interface IConsulConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets a <see cref="CancellationToken"/> that can be used to cancel any consul requests
        /// either loading or watching via long polling.
        /// It is recommended that this is cancelled during shut down.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> to be applied to the <see cref="ConsulClientConfiguration"/>
        /// during construction of the <see cref="IConsulClient"/>.
        /// Allows the default config options for Consul to be overriden.
        /// </summary>
        Action<ConsulClientConfiguration> ConsulConfigurationOptions { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> to be applied to the <see cref="HttpClient"/> during
        /// construction of the <see cref="IConsulClient"/>.
        /// Allows the default HTTP client options for Consul to be overriden.
        /// </summary>
        Action<HttpClient> ConsulHttpClientOptions { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> to be applied to the <see cref="HttpClientHandler"/>
        /// during construction of the <see cref="IConsulClient"/>.
        /// Allows the default HTTP client hander options for Consul to be overriden.
        /// </summary>
        Action<HttpClientHandler> ConsulHttpClientHandlerOptions { get; set; }

        /// <summary>
        /// Gets the key in Consul where the configuration is located.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is invoked when an exception is raised during config load.
        /// Used by clients to handle the exception if possible and prevent it from being thrown.
        /// </summary>
        Action<ConsulLoadExceptionContext> OnLoadException { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is invoked when an exception is raised whilst watching.
        /// Used by clients to acknowledge the excetion and possibly cancel the watcher.
        /// </summary>
        Action<ConsulWatchExceptionContext> OnWatchException { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the config is optional.
        /// </summary>
        bool Optional { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IConfigurationParser"/> to use when parsing the config.
        /// Allows different data formats to be stored in consul under the given key.
        /// Defaults to <see cref="JsonConfigurationParser"/>.
        /// </summary>
        IConfigurationParser Parser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source will be reloaded if the data in consul changes.
        /// </summary>
        bool ReloadOnChange { get; set; }
    }
}