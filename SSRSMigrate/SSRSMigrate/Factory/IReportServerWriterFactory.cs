
namespace SSRSMigrate.Factory
{
    public interface IReportServerWriterFactory
    {
        T GetWriter<T>(string name);
    }
}
