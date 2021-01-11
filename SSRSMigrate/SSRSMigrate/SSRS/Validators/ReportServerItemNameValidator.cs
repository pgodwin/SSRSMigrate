using Ninject;
using Ninject.Extensions.Logging;

namespace SSRSMigrate.SSRS.Validators
{
    //TODO Need to do actual checking of each item name as well.
    public class ReportServerItemNameValidator : IReportServerPathValidator
    {
        private ILogger mLogger = null;
        private string mInvalidPathChars = ":?;@&=+$,\\*><|.\"/";
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

        public bool Validate(string path)
        {
            bool isValidPath = true;

            this.mLogger.Debug("Validate - path = {0}", path);

            if (string.IsNullOrEmpty(path))
                isValidPath = false;
            else if (path.IndexOfAny(this.mInvalidPathChars.ToCharArray()) >= 0)
                isValidPath = false;
            else if (path.Length > this.mPathMaxLength)
                isValidPath = false;

            this.mLogger.Debug("Validate - isValidPath = {0}", isValidPath);

            return isValidPath;
        }
    }
}
