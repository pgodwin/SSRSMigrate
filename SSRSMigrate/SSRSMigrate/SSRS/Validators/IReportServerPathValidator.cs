namespace SSRSMigrate.SSRS.Validators
{
    public interface IReportServerPathValidator
    {
        string InvalidPathChars { get; }

        bool Validate(string path);
    }
}