using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS.Errors
{
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
