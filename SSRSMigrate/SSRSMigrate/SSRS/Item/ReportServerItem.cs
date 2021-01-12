using System;

namespace SSRSMigrate.SSRS.Item
{
    public class ReportServerItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public string ID { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Size { get; set; }
        public string VirtualPath { get; set; }

        public string ParentPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    throw new ArgumentException("item.Path");

                if (string.IsNullOrEmpty(this.Name))
                    throw new ArgumentException("item.Name");

                if (!string.IsNullOrEmpty(this.Path) && !string.IsNullOrEmpty(this.Name))
                {
                    string path = this.Path;
                    string name = this.Name;

                    string parentPath = null;
                    if (path == "/")
                        parentPath = path;
                    else
                    {
                        parentPath = path.Substring(0, path.LastIndexOf(name));

                        if (parentPath.EndsWith("/"))
                        {
                            parentPath = parentPath.Substring(0, parentPath.Length - 1);

                            if (parentPath.EndsWith("/"))
                                parentPath = parentPath.Substring(0, parentPath.Length - 1);
                        }
                    }

                    if (!parentPath.StartsWith("/"))
                        parentPath = "/" + parentPath;

                    return parentPath;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has valid Name and Path properties.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has valid properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidProperties
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Path))
                    return false;
                else
                    return true;
            }
        }

        public bool ShouldSerializeHasValidProperties()
        {
            // Prevent HasValidProperties from being serialized
            return false;
        }

        public bool ShouldSerializeParentPath()
        {
            // Prevent ParentPath from being serialized
            return false;
        }
    }
}
