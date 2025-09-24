using System.Security.Cryptography;
using System.Text;

namespace TransactionApi.Utils
{
    public static class HashFunctions
    {
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string ComputeMd5Hash(string rawData)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
