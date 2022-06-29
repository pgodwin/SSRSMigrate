using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{

    /// <summary>
    /// Simplified representation of an SSRS Schedule, which has a complex type system. Used by Snapshots, Subscriptions, Cache Refreshes
    /// </summary>
    public class SSRSScheduleDefinition : BaseSSRSItem
    {
        public ScheduleType ScheduleType { get; set; }

        public SchedulePatterns PatternType { get; set; }

        /// <summary>
        ///  Only relevant on a shared schedule reference
        /// </summary>
        public string ScheduleReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of a schedule.
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of a schedule.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the intervals at which a scheduled report runs. Intervals are specified in days.
        /// </summary>
        public int? DaysInterval { get; set; }

        /// <summary>
        /// An Integer value representing the interval, in minutes, at which a scheduled report runs.
        /// </summary>
        public int? MinutesInterval { get; set; }

        /// <summary>
        /// Gets or sets the days of the month on which a scheduled report runs.
        /// </summary>
        public string Days { get; set; }

        /// <summary>
        /// Represents the months of the year in which a scheduled report runs.

        /// </summary>
        public string[] Months { get; set; }

        /// <summary>
        /// Describes the week of the month on which a scheduled report runs.
        /// </summary>
        public int? WeekNum { get; set; }

        /// <summary>
        /// Represents the days of the week on which a scheduled report runs.
        /// </summary>
        public string[] DayOfWeek { get; set; }
        
        public int? WeeksInterval { get; set; }
    }


    public enum ScheduleType
    {
        NoSchedule,
        ScheduleReference,
        ScheduleDefinition
    }

    public enum SchedulePatterns
    {
        /// <summary>
        /// Represents the interval, in minutes, on which a scheduled report runs.
        /// </summary>
        MinuteRecurrence,

        /// <summary>
        /// Represents the interval, in days, on which a scheduled report runs.
        /// </summary>
        DailyRecurrence,

        /// <summary>
        /// Represents the weeks interval and the days of the week on which a scheduled report runs.
        /// </summary>
        WeeklyRecurrence,

        /// <summary>
        /// Represents the days of the month on which a scheduled report runs.
        /// </summary>
        MonthlyRecurrence,

        /// <summary>
        /// Represents the day of week, the week number in the month, and the month on which a scheduled report runs.
        /// </summary>
        MonthlyDOWRecurrence
    }
}
