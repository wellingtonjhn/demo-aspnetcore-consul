using System.Collections.Generic;
using System.IO;

namespace ConsulDemo.Helpers.Configuration.Parsers
{
    public interface IConfigurationParser
    {
        IDictionary<string, string> Parse(Stream stream);
    }
}