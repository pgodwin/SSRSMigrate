using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS.Errors
{
    public class InvalidItemException : Exception
    {
        public InvalidItemException()
            : base("InvalidItemException exception")
        {
        }

        public InvalidItemException(string Message)
            : base(Message)
        {
        }

        public InvalidItemException(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }

    public class InvalidPathException : Exception
    {
        public InvalidPathException()
            : base("InvalidPathException exception")
        {
        }

        public InvalidPathException(string Message)
            : base(Message)
        {
        }

        public InvalidPathException(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }

    public class InvalidReportDefinition : Exception
    {
        public InvalidReportDefinition()
            : base("InvalidReportDefinition exception")
        {
        }

        public InvalidReportDefinition(string Message)
            : base(Message)
        {
        }

        public InvalidReportDefinition(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }

    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException()
            : base("ItemAlreadyExistsException exception")
        {
        }

        public ItemAlreadyExistsException(string Message)
            : base(Message)
        {
        }

        public ItemAlreadyExistsException(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }

    public class FolderAlreadyExistsException : Exception
    {
        public FolderAlreadyExistsException()
            : base("FolderAlreadyExistsException exception")
        {
        }

        public FolderAlreadyExistsException(string Message)
            : base(Message)
        {
        }

        public FolderAlreadyExistsException(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }

    public class DataSourceAlreadyExistsException : Exception
    {
        public DataSourceAlreadyExistsException()
            : base("DataSourceAlreadyExistsException exception")
        {
        }

        public DataSourceAlreadyExistsException(string Message)
            : base(Message)
        {
        }

        public DataSourceAlreadyExistsException(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }

    public class ReportAlreadyExistsException : Exception
    {
        public ReportAlreadyExistsException()
            : base("ReportAlreadyExistsException exception")
        {
        }

        public ReportAlreadyExistsException(string Message)
            : base(Message)
        {
        }

        public ReportAlreadyExistsException(string Message, Exception inner)
            : base(Message, inner)
        {
        }
    }
}
