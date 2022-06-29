using Newtonsoft.Json;
using SSRSMigrate.SSRS.Item.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSRSMigrate.SSRS.Item
{
    /// <summary>
    /// Essientially a mapping of CatalogItem 
    /// </summary>
    [DebuggerDisplay("Type = {ItemType} / Name = {Name} / Path = {Path}")]
    public class ReportServerItem : 
        BaseSSRSItem, 
        IPolicies, 
        IProperties
    {
        /// <summary>
        /// Gets the name of an item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full path name of an item
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Gets/Sets the name of the user who originally added the item to the Report Server.
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets/Sets the date and time that the item was added to the Report Server.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The description of an item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ID of an item.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Gets the name of the user who last modified the item.
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Gets the date and time the user last modified the item.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// Gets the size, in bytes, of an item.
        /// </summary>
        public int? Size { get; set; }
        /// <summary>
        /// Gets the virtual path of the item.
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// An array of Property objects that represent the properties of the specified item.
        /// </summary>
        public List<ItemProperty> Properties { get; set; }

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

        /// <summary>
        /// Policies (or permissions/security settings) for the item.
        /// </summary>
        public List<PolicyDefinition> Policies { get; set; } = new List<PolicyDefinition>();

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
