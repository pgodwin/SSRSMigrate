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

        public InvalidItemException(string itemId)
            : base(string.Format("The item with ID '{0}' is invalid.", itemId))
        {
        }

        public InvalidItemException(string itemId, Exception inner)
            : base(itemId, inner)
        {
        }
    }

    public class InvalidPathException : Exception
    {
        public InvalidPathException()
            : base("InvalidPathException exception")
        {
        }

        public InvalidPathException(string itemPath)
            : base(string.Format("Invalid path '{0}'.", itemPath))
        {
        }

        public InvalidPathException(string itemPath, Exception inner)
            : base(itemPath, inner)
        {
        }
    }

    public class InvalidReportDefinitionException : Exception
    {
        public InvalidReportDefinitionException()
            : base("InvalidReportDefinition exception")
        {
        }

        public InvalidReportDefinitionException(string itemPath)
            : base(string.Format("Invalid report definition for report '{0}'.", itemPath))
        {
        }

        public InvalidReportDefinitionException(string itemPath, Exception inner)
            : base(itemPath, inner)
        {
        }
    }

    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException()
            : base("ItemAlreadyExistsException exception")
        {
        }

        public ItemAlreadyExistsException(string itemPath)
            : base(string.Format("An item at '{0}' already exists.", itemPath))
        {
        }

        public ItemAlreadyExistsException(string itemPath, Exception inner)
            : base(itemPath, inner)
        {
        }
    }
}
