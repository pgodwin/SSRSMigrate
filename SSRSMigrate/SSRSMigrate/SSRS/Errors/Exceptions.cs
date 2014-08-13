using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS.Errors
{
    public class InvalidPathCharsException : Exception
    {
        public InvalidPathCharsException()
            : base("InvalidPathCharsException exception")
        {
        }

        public InvalidPathCharsException(string Message)
            : base(Message)
        {
        }

        public InvalidPathCharsException(string Message, Exception inner)
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
}
