using System.IO;

namespace SSRSMigrate.Bundler
{
    public interface ICheckSumGenerator
    {
        string CreateCheckSum(string fileName);
        string CreateCheckSum(Stream stream);
    }
}
