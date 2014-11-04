using SSRSMigrate.SSRS.Repository;

namespace SSRSMigrate.Factory
{
    public interface IReportServerRepositoryFactory
    {
        IReportServerRepository GetRepository(string name);
    }
}
