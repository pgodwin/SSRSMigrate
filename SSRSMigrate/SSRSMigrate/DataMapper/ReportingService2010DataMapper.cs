using System;
using System.Collections.Generic;
using System.Linq;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.DataMapper
{
    public class ReportingService2010DataMapper : IDataMapper<
        ReportServer2010.CatalogItem,
        ReportServer2010.DataSourceDefinition,
        ReportServer2010.CatalogItem, // Dataset
        ReportServer2010.ItemReference,
        ReportServer2010.ItemReferenceData,
        ReportServer2010.QueryDefinition,
        ReportServer2010.Role,
        ReportServer2010.Policy,
        ReportServer2010.ScheduleDefinitionOrReference,
        ReportServer2010.Subscription,
        ReportServer2010.ScheduleDefinitionOrReference
        >
    {
        public DataSourceItem GetDataSource(CatalogItem item, DataSourceDefinition definition)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (definition == null)
                throw new ArgumentNullException("definition");

            DataSourceItem ds = new DataSourceItem();

            ds.Name = item.Name;
            ds.Path = item.Path;
            ds.CreatedBy = item.CreatedBy;
            ds.CreationDate = item.CreationDate;
            ds.Description = item.Description;
            ds.ID = item.ID;
            ds.ModifiedBy = item.ModifiedBy;
            ds.ModifiedDate = item.ModifiedDate;
            ds.Size = item.Size;
            ds.VirtualPath = item.VirtualPath;
            ds.ItemType = item.TypeName;

            ds.ConnectString = definition.ConnectString;

            switch (definition.CredentialRetrieval)
            {
                case CredentialRetrievalEnum.Integrated:
                    ds.CredentialsRetrieval = "Integrated"; break;
                case CredentialRetrievalEnum.None:
                    ds.CredentialsRetrieval = "None"; break;
                case CredentialRetrievalEnum.Prompt:
                    ds.CredentialsRetrieval = "Prompt"; break;
                case CredentialRetrievalEnum.Store:
                    ds.CredentialsRetrieval = "Store"; break;
            }

            ds.Enabled = definition.Enabled;
            ds.EnabledSpecified = definition.EnabledSpecified;
            ds.Extension = definition.Extension;
            ds.ImpersonateUser = definition.ImpersonateUser;
            ds.ImpersonateUserSpecified = definition.ImpersonateUserSpecified;
            ds.OriginalConnectStringExpressionBased = ds.OriginalConnectStringExpressionBased;
            ds.Password = definition.Password;
            ds.Prompt = definition.Prompt;
            ds.UseOriginalConnectString = definition.UseOriginalConnectString;
            ds.UserName = definition.UserName;
            ds.WindowsCredentials = definition.WindowsCredentials;

            ds.SourceObject = item;

            return ds;
        }

        public ReportItem GetReport(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            ReportItem report = new ReportItem();

            report.Name = item.Name;
            report.Path = item.Path;
            report.CreatedBy = item.CreatedBy;
            report.CreationDate = item.CreationDate;
            report.Description = item.Description;
            report.ID = item.ID;
            report.ModifiedBy = item.ModifiedBy;
            report.ModifiedDate = item.ModifiedDate;
            report.Size = item.Size;
            report.VirtualPath = item.VirtualPath;
            report.ItemType = item.TypeName;
            report.SourceObject = item;

            return report;
        }

        public FolderItem GetFolder(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            FolderItem folder = new FolderItem();

            folder.CreatedBy = item.CreatedBy;
            folder.CreationDate = item.CreationDate;
            folder.Description = item.Description;
            folder.ID = item.ID;
            folder.ModifiedBy = item.ModifiedBy;
            folder.ModifiedDate = item.ModifiedDate;
            folder.Name = item.Name;
            folder.Path = item.Path;
            folder.Size = item.Size;
            folder.VirtualPath = item.VirtualPath;
            folder.ItemType = item.TypeName;

            folder.SourceObject = item;

            return folder;
        }

        public DataSetItem GetDataSet(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.TypeName != "Dataset")
                throw new ArgumentException("Item.Type is not a dataset");
            var datasetItem = new DataSetItem();
            datasetItem.Name = item.Name;
            datasetItem.Path = item.Path;
            datasetItem.CreatedBy = item.CreatedBy;
            datasetItem.CreationDate = item.CreationDate;
            datasetItem.Description = item.Description;
            datasetItem.ID = item.ID;
            datasetItem.ModifiedBy = item.ModifiedBy;
            datasetItem.ModifiedDate = item.ModifiedDate;
            datasetItem.Size = item.Size;
            datasetItem.VirtualPath = item.VirtualPath;
            datasetItem.ItemType = item.TypeName;

            // TODO - handle properties?
            datasetItem.SourceObject = item;


            return datasetItem;
        }

        public ItemReferenceDefinition GetItemReference(ReportServer2010.ItemReference item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var itemReference = new ItemReferenceDefinition()
            {
                Name = item.Name,
                Reference = item.Reference,
                SourceObject = item
            };

            return itemReference;

        }

        public ItemReferenceDefinition GetItemReferenceData(ItemReferenceData item)
        {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var itemReference = new ItemReferenceDefinition()
            {
                Name = item.Name,
                Reference = item.Reference,
                ReferenceType = item.ReferenceType,
                SourceObject = item
            };

            return itemReference;
        }

        public SSRS.Item.QueryDefinition GetQueryDefinition(ReportServer2010.QueryDefinition item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var queryDefinition = new SSRS.Item.QueryDefinition()
            {
                SourceObject = item,
                CommandText = item.CommandText,
                CommandType = item.CommandType,
            };
            if (item.TimeoutSpecified)
                queryDefinition.Timeout = item.Timeout;
            else
                queryDefinition.Timeout = null;

            return queryDefinition;
        }

        public PolicyDefinition GetPolicy(Policy item, bool inherit)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var policyDefinition = new PolicyDefinition()
            {
                GroupUserName = item.GroupUserName,
                Roles = item.Roles.Select(GetRole).ToList(),
                SourceObject = item,
                InheritFromParent = inherit
            };

            return policyDefinition;
        }

        public RoleDefinition GetRole(Role item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var roleDefinition = new RoleDefinition()
            {
                SourceObject = item,
                Description = item.Description,
                Name = item.Name
            };

            return roleDefinition;
        }

        public ReportServerItem GetReportServerItem(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var serverItem = new ReportServerItem();
            serverItem.Name = item.Name;
            serverItem.Path = item.Path;
            serverItem.CreatedBy = item.CreatedBy;
            serverItem.CreationDate = item.CreationDate;
            serverItem.Description = item.Description;
            serverItem.ID = item.ID;
            serverItem.ModifiedBy = item.ModifiedBy;
            serverItem.ModifiedDate = item.ModifiedDate;
            serverItem.Size = item.Size;
            serverItem.VirtualPath = item.VirtualPath;
            serverItem.SourceObject = item;
            serverItem.ItemType = item.TypeName;

            return serverItem;

        }

        public SubscriptionDefinition GetSubscription(Subscription item)
        {
            var subscription = new SubscriptionDefinition()
            {
                SourceObject = item
            };
            throw new NotImplementedException();
            return subscription;
        }

        public SnapshotOptionsDefinition GetHistoryOptionsDefinition(ScheduleDefinitionOrReference item, bool keepSnapshots)
        {
            var options = new SnapshotOptionsDefinition();
            options.SourceObject = item;
            options.KeepExecutionSnapshots = keepSnapshots;

            
            return options;

        }
        private string[] ConvertMonthsOfYear(MonthsOfYearSelector selector)
        {
            var months = new List<string>();
            if (selector.January)
                months.Add("January");
            if (selector.February)
                months.Add("February");
            if (selector.March)
                months.Add("March");
            if (selector.April)
                months.Add("April");
            if (selector.May)
                months.Add("May");
            if (selector.June)
                months.Add("June");
            if (selector.July)
                months.Add("July");
            if (selector.August)
                months.Add("August");
            if (selector.September)
                months.Add("September");
            if (selector.October)
                months.Add("October");
            if (selector.November)
                months.Add("November");
            if (selector.December)
                months.Add("December");
            return months.ToArray();
        }

        private string[] ConvertDaysOfWeek(DaysOfWeekSelector selector)
        {
            var daysOfWeek = new List<string>();
            if (selector.Sunday)
                daysOfWeek.Add("Sunday");
            if (selector.Monday)
                daysOfWeek.Add("Monday");
            if (selector.Tuesday)
                daysOfWeek.Add("Tuesday");
            if (selector.Wednesday)
                daysOfWeek.Add("Wednesday");
            if (selector.Thursday)
                daysOfWeek.Add("Thursday");
            if (selector.Friday)
                daysOfWeek.Add("Friday");
            if (selector.Saturday)
                daysOfWeek.Add("Saturday");

            return daysOfWeek.ToArray();
        }

        public SSRS.Item.SSRSScheduleDefinition GetSchedule(ReportServer2010.ScheduleDefinitionOrReference item)
        {
            SSRSScheduleDefinition options = new SSRSScheduleDefinition();
            ScheduleDefinition definition = default;
            if (item is NoSchedule)
            {
                var noSchedule = item as NoSchedule;
                options.ScheduleType = ScheduleType.NoSchedule;
            }
            if (item is ScheduleReference)
            {
                var scheduleReference = item as ScheduleReference;
                options.ScheduleType = ScheduleType.ScheduleReference;
                options.ScheduleReferenceId = scheduleReference.ScheduleID;
                definition = scheduleReference.Definition;
            }
            if (item is ScheduleDefinition)
            {
                definition = (ScheduleDefinition)item;
                options.ScheduleType = ScheduleType.ScheduleDefinition;
            }
            if (definition == null)
                return options;

            // At this point we have a definition defined - handle the mapping back to this item
            options.StartDateTime = definition.StartDateTime;
            if (definition.EndDateSpecified)
                options.EndDateTime = definition.EndDate;
            else
                options.EndDateTime = null;

            // Handle the patterns
            var pattern = definition.Item;
            //
            if (pattern is DailyRecurrence)
            {
                var daily = pattern as DailyRecurrence;
                options.PatternType = SchedulePatterns.DailyRecurrence;
                options.DaysInterval = daily.DaysInterval;
            }
            if (pattern is MinuteRecurrence)
            {
                var minutes = pattern as MinuteRecurrence;
                options.PatternType = SchedulePatterns.MinuteRecurrence;
                options.MinutesInterval = minutes.MinutesInterval;
            }
            if (pattern is WeeklyRecurrence)
            {
                var weeks = pattern as WeeklyRecurrence;
                options.PatternType = SchedulePatterns.WeeklyRecurrence;
                if (weeks.WeeksIntervalSpecified)
                    options.WeeksInterval = weeks.WeeksInterval;

                options.DayOfWeek = ConvertDaysOfWeek(weeks.DaysOfWeek);


            }
            if (pattern is MonthlyRecurrence)
            {
                var monthly = pattern as MonthlyRecurrence;
                options.PatternType = SchedulePatterns.MonthlyRecurrence;
                options.Days = monthly.Days;
                options.Months = ConvertMonthsOfYear(monthly.MonthsOfYear);
            }
            if (pattern is MonthlyDOWRecurrence)
            {
                var dow = pattern as MonthlyDOWRecurrence;
                options.PatternType = SchedulePatterns.MonthlyDOWRecurrence;
                options.DayOfWeek = ConvertDaysOfWeek(dow.DaysOfWeek);
                options.Months = ConvertMonthsOfYear(dow.MonthsOfYear);
                if (dow.WhichWeekSpecified)
                    options.WeekNum = (int)dow.WhichWeek;
            }

            return options;

        }
    }

   
}
