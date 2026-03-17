using System.Security.Cryptography;
using System.Text;

namespace TransactionApi.Utils
{
    public static class HashFunctions
    {
        public static string ComputeMD5Hash(string rawData)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string ComputeSHA1Hash(string rawData)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string ComputeSHA256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string ComputeSHA384Hash(string rawData)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                var hash = sha384.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string ComputeSHA512Hash(string rawData)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
