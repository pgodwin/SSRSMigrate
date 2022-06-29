using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    /// <summary>
    /// Support for Snapshot Configuration
    /// </summary>
    public interface ISnapshotOptions
    {
        SnapshotOptionsDefinition SnapshotOptions { get; set; }
    }
}
