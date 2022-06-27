using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Attributes
{
    [Serializable]
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class DefaultEndpointAttribute : Attribute
    {
        public DefaultEndpointAttribute(string defaultEndpoint)
        {
            this.DefaultEndpoint = defaultEndpoint;
        }

        public string DefaultEndpoint { get; private set; }
    }
}
