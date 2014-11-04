using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Importer
{
    public interface IItemImporter<T> where T : ReportServerItem
    {
        T ImportItem(string filename, out ImportStatus status);
    }
}
