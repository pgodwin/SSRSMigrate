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

    public class InvalidReportDefinitionException : Exception
    {
        public InvalidReportDefinitionException()
            : base("InvalidReportDefinition exception")
        {
        }

        public InvalidReportDefinitionException(string Message)
            : base(Message)
        {
        }

        public InvalidReportDefinitionException(string Message, Exception inner)
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
}
