using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;

namespace SSRSMigrate.SSRS.Test
{
    //TODO Write tests
    public class ReportServerTester : IReportServerTester
    {
        private readonly IReportServerRepository mReportRepository;
        private readonly ILogger mLogger = null;

        public ReportServerTester(IReportServerRepository repository, ILogger logger)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mReportRepository = repository;
            this.mLogger = logger;
        }

        public ConnectionTestStatus ReadTest(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Info("Testing read using connection to '{0}' using path '{1}'...", 
                this.mReportRepository.ServerAddress,
                path);

            ConnectionTestStatus status = new ConnectionTestStatus();
            status.ServerAddress = this.mReportRepository.ServerAddress;
            status.Path = path;

            try
            {
                bool exists = this.mReportRepository.ItemExists(path, "Folder");

                if (exists)
                    status.Success = true;
                else
                {
                    status.Success = false;
                    status.Error = "Item not returned.";
                }

                return status;
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "Read test failed.");

                status.Success = false;
                status.Error = er.Message;

                return status;
            }
        }

        public ConnectionTestStatus WriteTest(string path, string itemName)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentException("itemName");

            this.mLogger.Info("Testing write using connection to '{0}' using path '{1}'...",
                this.mReportRepository.ServerAddress,
                path);

            ConnectionTestStatus status = new ConnectionTestStatus();
            status.ServerAddress = this.mReportRepository.ServerAddress;
            status.Path = path;

            try
            {
                string error = this.mReportRepository.CreateFolder(itemName, path);

                if (string.IsNullOrEmpty(error))
                {
                    status.Success = true;

                    this.mReportRepository.DeleteItem(path + itemName);
                }
                else
                {
                    status.Success = false;
                    status.Error = error;
                }

                return status;
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "Write test failed.");

                status.Success = false;
                status.Error = er.Message;

                return status;
            }
        }
    }
}
