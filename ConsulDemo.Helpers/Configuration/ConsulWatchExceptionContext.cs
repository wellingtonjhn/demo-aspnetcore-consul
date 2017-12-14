using ConsulDemo.Helpers.Configuration.Contracts;
using System;

namespace ConsulDemo.Helpers.Configuration
{
    public sealed class ConsulWatchExceptionContext
    {
        public Exception Exception { get; }
        public IConsulConfigurationSource Source { get; }

        internal ConsulWatchExceptionContext(IConsulConfigurationSource source, Exception exception)
        {
            Exception = exception;
            Source = source;
        }
    }
}