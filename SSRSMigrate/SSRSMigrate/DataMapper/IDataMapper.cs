using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.DataMapper
{
    public interface IDataMapper<T, U>
    {
        DataSourceItem GetDataSource(T item, U definition);
        ReportItem GetReport(T item, byte[] definition);
        FolderItem GetFolder(T item);
    }
}
