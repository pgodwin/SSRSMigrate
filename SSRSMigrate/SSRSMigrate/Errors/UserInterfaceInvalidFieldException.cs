using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Errors
{
    public class UserInterfaceInvalidFieldException : Exception
    {
        public UserInterfaceInvalidFieldException()
            : base("UserInterfaceInvalidFieldException exception")
        {
        }

        public UserInterfaceInvalidFieldException(string fieldName)
            : base(string.Format("Field '{0}' contains an invalid value.", fieldName))
        {
        }

        public UserInterfaceInvalidFieldException(string itemId, Exception inner)
            : base(itemId, inner)
        {
        }
    }
}
