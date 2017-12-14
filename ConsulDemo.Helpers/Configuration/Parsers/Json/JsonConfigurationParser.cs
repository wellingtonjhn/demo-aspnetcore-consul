using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace ConsulDemo.Helpers.Configuration.Parsers.Json
{
    public sealed class JsonConfigurationParser : IConfigurationParser
    {
        public IDictionary<string, string> Parse(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                jsonReader.DateParseHandling = DateParseHandling.None;
                var jsonConfig = JObject.Load(jsonReader);
                return jsonConfig.Flatten();
            }
        }
    }
}