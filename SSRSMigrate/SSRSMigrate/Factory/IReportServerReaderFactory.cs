

namespace SSRSMigrate.Factory
{
    public interface IReportServerReaderFactory
    {
        T GetReader<T>(string name);
    }
}
