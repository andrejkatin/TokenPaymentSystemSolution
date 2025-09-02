using System.Text;

namespace TokenApi.Utils.Extensions
{
    public static class ConvertStringToByteArray
    {
        public static byte[] ToByteArray(this string value)
        {        
            // Use UTF8 encoding to convert the string to a byte array
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
