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
    }
}
