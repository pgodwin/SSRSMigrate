using Ninject;
using Ninject.Extensions.Logging;

namespace SSRSMigrate.SSRS.Validators
{
    public class ReportServerPathValidator : IReportServerPathValidator
    {
        private ILogger mLogger = null;
        private string mInvalidPathChars = ":?;@&=+$,\\*><|.\"";
        private string mInvalidNameChars = ":?;@&=+$,\\*><|.\"/";
        private int mPathMaxLength = 260;

        [Inject]
        public ILogger Logger
        {
            get { return this.mLogger; }
            set { this.mLogger = value; }
        }
        public string InvalidPathChars
        {
            get { return this.mInvalidPathChars; }
        }

        public string InvalidNameChars
        {
            get { return this.mInvalidNameChars; }
        }

        public bool Validate(string path)
        {
            bool isValidPath = true;

            this.mLogger.Debug("Validate - name = {0}", path);

            if (string.IsNullOrEmpty(path))
                isValidPath = false;
            else if (path.IndexOfAny(this.mInvalidPathChars.ToCharArray()) >= 0)
                isValidPath = false;
            else if (path.Length > this.mPathMaxLength)
                isValidPath = false;

            this.mLogger.Debug("Validate - isValidPath = {0}", isValidPath);

            return isValidPath;
        }

        public bool ValidateName(string name)
        {
            bool isValidPath = true;

            this.mLogger.Debug("ValidateName - name = {0}", name);

            if (string.IsNullOrEmpty(name))
                isValidPath = false;
            else if (name.IndexOfAny(this.mInvalidNameChars.ToCharArray()) >= 0)
                isValidPath = false;
            else if (name.Length > this.mPathMaxLength)
                isValidPath = false;

            this.mLogger.Debug("ValidateName - isValidPath = {0}", isValidPath);

            return isValidPath;
        }
    }
}
