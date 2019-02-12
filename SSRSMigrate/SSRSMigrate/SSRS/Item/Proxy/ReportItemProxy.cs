using SSRSMigrate.SSRS.Repository;

namespace SSRSMigrate.SSRS.Item.Proxy
{
    public class ReportItemProxy : ReportItem
    {
        private byte[] mDefinition = null;
        private readonly IReportServerRepository mReportServerRepository;

        public ReportItemProxy(IReportServerRepository repository)
        {
            this.mReportServerRepository = repository;
        }

        public override byte[] Definition
        {
            get
            {
                if (this.mDefinition == null)
                {
                    byte[] data = this.mReportServerRepository.GetReportDefinition(this.Path);

                    this.mDefinition = data;

                    return data;
                }
                else
                {
                    return this.mDefinition;
                }
            }

            set { this.mDefinition = value; }
        }
    }
}
