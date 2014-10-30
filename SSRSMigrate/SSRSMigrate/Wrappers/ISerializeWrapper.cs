using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Wrappers
{
    public interface ISerializeWrapper
    {
        T DeserializeObject<T>(string value);
        string SerializeObject(object value);
    }
}
