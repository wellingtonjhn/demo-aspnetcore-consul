using ConsulDemo.Helpers.Configuration.Contracts;
using System;

namespace ConsulDemo.Helpers.Configuration
{
    public sealed class ConsulLoadExceptionContext
    {
        public Exception Exception { get; }
        public bool Ignore { get; set; }
        public IConsulConfigurationSource Source { get; }

        internal ConsulLoadExceptionContext(IConsulConfigurationSource source, Exception exception)
        {
            Source = source;
            Exception = exception;
        }
    }
}