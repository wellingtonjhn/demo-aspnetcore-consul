using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ConsulDemo.Helpers.Configuration.Parsers.Json
{
    internal static class JsonFlattener
    {
        internal static IDictionary<string, string> Flatten(this JObject jObject)
        {
            var jsonPrimitiveVisitor = new JsonPrimitiveVisitor();
            IDictionary<string, string> data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var primitive in jsonPrimitiveVisitor.VisitJObject(jObject))
            {
                if (data.ContainsKey(primitive.Key))
                {
                    throw new FormatException($"Key {primitive.Key} is duplicated in json");
                }

                data.Add(primitive);
            }
            return data;
        }
    }
}