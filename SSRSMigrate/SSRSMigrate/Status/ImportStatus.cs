using System;

namespace SSRSMigrate.Status
{
    public class ImportStatus
    {
        private string mFromFileName;
        private string mFromPath;
        private Exception mError;
        private bool mSuccess;

        public string FromFileName
        {
            get { return mFromFileName; }
            protected set { mFromFileName = value; }
        }

        public string FromPath
        {
            get { return mFromPath; }
            protected set { mFromPath = value; }
        }

        public Exception Error
        {
            get { return mError; }
            protected set { mError = value; }
        }

        public bool Success
        {
            get { return mSuccess; }
            protected set { mSuccess = value; }
        }

        public ImportStatus(
            string fromFileName,
            string fromPath,
            Exception error,
            bool success)
        {
            if (string.IsNullOrEmpty(fromFileName))
                throw new ArgumentException("patfromFileNamehTo");

            if (string.IsNullOrEmpty(fromPath))
                throw new ArgumentException("fromPath");

            this.mFromFileName = fromFileName;
            this.mFromPath = fromPath;
            this.mError = error;
            this.mSuccess = success;
        }
    }
}
