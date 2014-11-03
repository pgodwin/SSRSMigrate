using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Errors
{
    public class InvalidFileArchiveException : Exception
    {
        public InvalidFileArchiveException()
            : base("InvalidFileArchiveException exception")
        {
        }

        public InvalidFileArchiveException(string fieldName)
            : base(string.Format("'{0}' is not a valid archive.", fieldName))
        {
        }

        public InvalidFileArchiveException(string itemID, Exception inner)
            : base(itemID, inner)
        {
        }
    }
}
