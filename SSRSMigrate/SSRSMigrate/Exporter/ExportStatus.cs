using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Exporter
{
    public class ExportStatus
    {
        private string mToPath;
        private string mFromPath;
        private string[] mErrors;
        private bool mSuccess;

        /// <summary>
        /// Gets the full path to where the item was exported.
        /// </summary>
        /// <value>
        /// Full path on disk.
        /// </value>
        public string ToPath 
        {
            get { return this.mToPath; }
            protected set { this.mToPath = value; }
        }

        /// <summary>
        /// Gets the full path to where the item was located before it was exported.
        /// </summary>
        /// <value>
        /// From path.
        /// </value>
        public string FromPath
        {
            get { return this.mFromPath; }
            protected set { this.mFromPath = value; }
        }

        /// <summary>
        /// Gets the errors, if any, that occurred during the export process.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public string[] Errors
        {
            get { return this.mErrors; }
            protected set { this.mErrors = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ExportStatus"/> is for a successful export.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get { return this.mSuccess; }
            protected set { this.mSuccess = value; }
        }
    }
}
