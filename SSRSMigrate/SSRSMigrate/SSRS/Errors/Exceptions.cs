using System;

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

    public class InvalidItemTypeException : Exception 
    {
        public InvalidItemTypeException() : base("InvalidItemTypeException") { }

        public InvalidItemTypeException(string path, string itemType, string expectedType) : base($"The itemType does not match the expected type. Received '{itemType}', expected '{expectedType}'. Item path '{path}") 
        {
            this.Path = path;
            this.ItemType = itemType;
            this.ExpectedType = expectedType;
        }

        public InvalidItemTypeException(string path, string itemType, string expectedType, Exception inner) : base($"The itemType does not match the expected type. Received '{itemType}', expected '{expectedType}'. Item path '{path}", inner) 
        {
            this.Path = path;
            this.ItemType = itemType;
            this.ExpectedType = expectedType;
        }

        public string Path { get; }
        public string ItemType { get; }
        public string ExpectedType { get; }
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

    public class InvalidDatasetDefinitionException : Exception
    { 
        public InvalidDatasetDefinitionException()
             : base(nameof(InvalidDatasetDefinitionException))
        {
        }

        public InvalidDatasetDefinitionException(string itemPath)
            : base($"Invalid Dataset Definition for path '{itemPath}'") 
        { 
        }

        public InvalidDatasetDefinitionException(string itemPath, Exception inner)
            : base($"Invalid Dataset Definition for path '{itemPath}'", inner)
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
