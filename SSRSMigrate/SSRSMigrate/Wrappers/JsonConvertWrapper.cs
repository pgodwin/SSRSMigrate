using System;
using Newtonsoft.Json;

namespace SSRSMigrate.Wrappers
{
    public class JsonConvertWrapper : ISerializeWrapper
    {
        public T DeserializeObject<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("value");

            return JsonConvert.DeserializeObject<T>(value);
        }

        public string SerializeObject(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
    }
}
