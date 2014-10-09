using System;
using System.IO;
using System.Security.Cryptography;

namespace SSRSMigrate.Bundler
{
    public class MD5CheckSumGenerator : ICheckSumGenerator
    {
        public string CreateCheckSum(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (Directory.Exists(fileName))
                return "";

            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            //if (!File.Exists(fileName))
            //    return "";

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public string CreateCheckSum(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
            }
        }
    }
}
