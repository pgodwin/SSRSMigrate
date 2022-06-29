
using SSRSMigrate.SSRS.Item.Interfaces;
using System.Diagnostics;

namespace SSRSMigrate.SSRS.Item
{
    [DebuggerDisplay("Name = {Name}")]
    public class FolderItem : 
        ReportServerItem,
        IPolicies,
        IProperties
    {
        public FolderItem()
        {

        }
    }
}
