using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    public interface ISubscriptions
    {
        // Support for Subscription
        List<SubscriptionDefinition> Subscriptions { get; set; }
    }
}
