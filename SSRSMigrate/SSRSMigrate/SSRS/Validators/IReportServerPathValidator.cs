using System.Net;

namespace SSRSMigrate.SSRS.Validators
{
    public interface IReportServerPathValidator
    {
        string InvalidPathChars { get; }
        string InvalidNameChars { get; }

        bool Validate(string path);
        bool ValidateName(string name);
    }
}