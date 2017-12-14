namespace ConsulDemo.Helpers.Configuration.Contracts
{
    /// <summary>The result of a query for config in Consul.</summary>
    internal interface IConfigQueryResult
    {
        /// <summary>Gets a value indicating whether the config exists.</summary>
        bool Exists { get; }

        /// <summary>Gets the raw value of the config.</summary>
        byte[] Value { get; }
    }
}