using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public class PolicyDefinition : BaseSSRSItem
    {
        public string GroupUserName { get; set; }
        public List<RoleDefinition> Roles { get; set; }

        public bool InheritFromParent { get; set; }

        public PolicyDefinition()
        {
            this.Roles = new List<RoleDefinition>();
        }
    }
}
